How to use SkyViewerService:
Remark£º
  This service will be create when install Skyviewer software,
and will be deleted when uninstall Skyviewer software.

  To debug this project,you can run the following command
(1)Create service
sc create SkyviewService binPath= "Path to SkyViewerService.exe"

(1)Start service
sc start SkyviewService

(1)Query service state
sc query SkyviewService

(1)Stop service
sc stop SkyviewService

(1)Delete Service
sc delete SkyviewService
