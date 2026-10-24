#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException')

## CurlFileException\.FileOperation Enum

The kind of file access that failed, used by [Operation](CurlDotNet.Exceptions.CurlFileException.Operation.md 'CurlDotNet\.Exceptions\.CurlFileException\.Operation') to choose the advice in the message\.

```csharp
public enum CurlFileException.FileOperation
```
### Fields

<a name='CurlDotNet.Exceptions.CurlFileException.FileOperation.Read'></a>

`Read` 0

Reading an existing file \(for example a request body for `-d @file`\) failed: missing file,
no read permission, or the file is locked\.

### Remarks

Check the path and the permissions of the current user.

### See Also
- [Upload](CurlDotNet.Exceptions.CurlFileException.FileOperation.md#CurlDotNet.Exceptions.CurlFileException.FileOperation.Upload 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation\.Upload')

<a name='CurlDotNet.Exceptions.CurlFileException.FileOperation.Write'></a>

`Write` 1

Writing to a file \(for example a response saved with `-o`\) failed: no write permission, disk
full, or the folder does not exist\.

### Remarks

Check the folder exists (or use `--create-dirs`) and the disk has space.

### See Also
- [Create](CurlDotNet.Exceptions.CurlFileException.FileOperation.md#CurlDotNet.Exceptions.CurlFileException.FileOperation.Create 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation\.Create')

<a name='CurlDotNet.Exceptions.CurlFileException.FileOperation.Create'></a>

`Create` 2

Creating a new file failed: the folder is missing, read\-only, or a file with that name is locked\.

### Remarks

Create the folder first or add `--create-dirs`.

### See Also
- [Write](CurlDotNet.Exceptions.CurlFileException.FileOperation.md#CurlDotNet.Exceptions.CurlFileException.FileOperation.Write 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation\.Write')

<a name='CurlDotNet.Exceptions.CurlFileException.FileOperation.Delete'></a>

`Delete` 3

Deleting a file \(for example a partial download after `--remove-on-error`\) failed because it is
locked or read\-only\.

### Remarks

Close programs that hold the file open and check its attributes.

### See Also
- [Write](CurlDotNet.Exceptions.CurlFileException.FileOperation.md#CurlDotNet.Exceptions.CurlFileException.FileOperation.Write 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation\.Write')

<a name='CurlDotNet.Exceptions.CurlFileException.FileOperation.Upload'></a>

`Upload` 4

Reading a file to upload it \(`-T file` or `-F name=@file`\) failed: missing file or no read
permission\.

### Remarks

See the upload guide linked from [CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException').

### See Also
- [Read](CurlDotNet.Exceptions.CurlFileException.FileOperation.md#CurlDotNet.Exceptions.CurlFileException.FileOperation.Read 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation\.Read')

<a name='CurlDotNet.Exceptions.CurlFileException.FileOperation.Download'></a>

`Download` 5

Saving a downloaded file \(`-o file` or `-O`\) failed: the folder is missing, not writable, or
the disk is full\.

### Remarks

See the download guide linked from [CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException').

### See Also
- [Write](CurlDotNet.Exceptions.CurlFileException.FileOperation.md#CurlDotNet.Exceptions.CurlFileException.FileOperation.Write 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation\.Write')

### Remarks

Read/Upload point at the upload guide, the others at the download guide.

### See Also
- [Operation](CurlDotNet.Exceptions.CurlFileException.Operation.md 'CurlDotNet\.Exceptions\.CurlFileException\.Operation')