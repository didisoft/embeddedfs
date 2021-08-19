# embeddedfs
Embedded File System for .NET, contained in a single file or in-memory.

![EmbeddedFS API](data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==)

<noscript>![EmbeddedFS API](https://www.didisoft.com/wp-content/uploads/2021/03/EmbeddedFS.png)</noscript>

DidiSoft [EmbeddedFS](/embeddedfs/) is a **file system** located **in memory** or **in a single file**. This is the starting point for getting familiar with the EmbeddedFS API.

**Contents**

*   [Setup](/embeddedfs/examples/setup/)
*   [Create/Open the embedded file system](/embeddedfs/examples/drive/).
*   [Working with folders](/embeddedfs/examples/folder/)
*   [Working with files](/embeddedfs/examples/file/)

### Key Members of EmbeddedFS namespace

<table class="table table-hover">

<tbody>

<tr>

<td>[EmDriveInfo](/embeddedfs/examples/drive/)</td>

<td>[Create/Open](/embeddedfs/examples/drive/) an embedded file system</td>

</tr>

<tr>

<td>EmDrivectory</td>

<td>Provides _static methods_ for [manipulating directories](/embeddedfs/examples/folder/) contained in **EmDriveInfo**</td>

</tr>

<tr>

<td>[EmDrivectoryInfo](/embeddedfs/examples/folder/)</td>

<td>Exposes similar API as **EmDrivectory**, but from a valid _object reference_</td>

</tr>

<tr>

<td>EmFile</td>

<td>Provides _static methods_ for manipulating files contained in **EmDriveInfo**</td>

</tr>

<tr>

<td>[EmFileInfo](/embeddedfs/examples/file/)</td>

<td>Exposes similar API as **EmFile**, but from a valid _object reference_</td>

</tr>

</tbody>

</table>
