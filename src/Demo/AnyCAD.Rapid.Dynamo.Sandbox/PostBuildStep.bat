set DynamoRuntime=E:\Soft\DynamoCoreRuntime_3.0.3.7597
set Output=%1

echo DynamoRuntime:%DynamoRuntime%
echo BinaryOutput:%Output%

xcopy %DynamoRuntime% %Output% /e /y /d /exclude:ExcludedFiles.txt