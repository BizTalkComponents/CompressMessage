[![Build status](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/CompressMessage?branch=master)](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/CompressMessage/branch/master)

##Description
CompressMessage is used to compress multipart messages. The component iterates over all message parts adding each one to a zip archive.

The zip entry file names are set by the following logic:

1. The ReceivedFileName context property is used if set on the current message part.
2. Otherwise a guid is used and file name extension is set by:
  1. If ContentType it is used to map mime types to file extensions (application/pdf =>.pdf etc. ).
  2. If a default file extension is set in the component parameters it is used.
  3. Otherwise no file extension is set.


SharpLib zib library is used.


| Parameter                    | Description                                                               | Type| Validation|
| -----------------------------|---------------------------------------------------------------------------|-----|--------|
|DefaultZipEntryFileExtension|The default file extension to use if no content type is set on the message part.|String|Optional,RegEx pattern [a-zA-Z0-9]{3} |
