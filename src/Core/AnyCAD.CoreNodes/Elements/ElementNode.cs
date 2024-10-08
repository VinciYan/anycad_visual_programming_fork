﻿using Autodesk.DesignScript.Runtime;

using AnyCAD.Foundation;

using AnyCAD.Rapid.Dynamo.Services.Elements;
using AnyCAD.Rapid.Dynamo.Services.Persistence;
using AnyCADWpfCommon;

namespace AnyCAD.CoreNodes.Elements
{
    /// <summary>
    /// ElementNode，AnyCAD.Foundation.Element在Dynamo环境下的适配器
    /// 实现了与AnyCAD文档对象之间的绑定和生命周期管理
    /// </summary>
    public abstract class ElementNode : IDisposable
    {
        protected void SafeInit(Action init)
        {
            try
            {
                init();
            }
            catch (Exception e)
            {
                // TODO
                throw e;
            }
        }

        internal static Document Document => Application.Instance().GetActiveDocument();

        /// <summary>
        /// 若当前对象已经被anycad所拥有，那么在Dispose中不应被删除
        /// </summary>
        internal bool mIsOwned = false;

        [SupressImportIntoVM]
        public abstract Element InternalElement { get; }

        private ElementId mInternalId;

        protected ElementId InternalId
        {
            get
            {
                if (mInternalId == null || !mInternalId.IsValid())
                    return InternalElement != null ? InternalElement.GetId() : ElementId.InvalidId;
                return mInternalId;
            }
            set
            {
                mInternalId = value;

                var manager = ElementIdLifecycleManager.Instance;
                manager.RegisterAsssociation(Id, this);
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public virtual void Dispose()
        {
            if (DisposeLogic.IsShuttingDown || DisposeLogic.IsClosingHomeworkspace)
                return;

            var manager = ElementIdLifecycleManager.Instance;
            // bool didOwnerDelete = manager.IsOwnerDeleted(Id);

            int remainingBindings = manager.UnRegisterAssociation(Id, this);

            if (!mIsOwned && remainingBindings == 0 && InternalId.IsValid())
            {
                WorkManager.Instance.RemoveNode(InternalId);
            }
            else
            {
                //This element has gone
                //but there was something else holding onto the AnyCAD object so don't purge it

                mInternalId = ElementId.InvalidId;
            }
        }

        public ulong Id
        {
            get
            {
                var id = InternalId;
                if (!id.IsValid())
                    return 0;
                return id.GetInteger();
            }
        }
    }
}
