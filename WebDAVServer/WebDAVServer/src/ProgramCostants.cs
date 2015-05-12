namespace WebDAVServer {
    static class ProgramCostants {
        internal const int DefautServerPort = 27999;
        internal const long MinProgressViewingSize = 10 * 1024 * 1024;
        internal const long DefautBufferSize = 1024 * 1024;
        internal const bool Debug = true;
    }

    static class HttpStatusCodes {

        internal const int InfoContinue = 100;

        internal const int SuccessOk = 200;
        internal const int SuccessCreated = 201;
        internal const int SuccessNoContent = 204;
        internal const int SuccessMultistatus = 207;

        internal const int ClientErrorBadRequest = 400;
        internal const int ClientErrorForbidden = 403;
        internal const int ClientErrorNotFound = 404;
        internal const int ClientErrorMethodNotAllowed = 405;
        internal const int ClientErrorPreconditionFailed = 412;
        internal const int ClientErrorLocked = 423;
        internal const int ClientErrorFailedDependency = 424;

        internal const int ServerErrorInternalError = 500;
    }
}
