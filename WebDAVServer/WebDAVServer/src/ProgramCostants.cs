namespace WebDAVServer {
    static class ProgramCostants {
        internal const int DEFAUT_SERVER_PORT = 27999;
        internal const long MIN_PROGRESS_VIEWING_SIZE = 10 * 1024 * 1024;
        internal const long DEFAUT_BUFFER_SIZE = 1024 * 1024;
        internal const bool DEBUG = true;
    }

    static class HttpStatusCodes {

        internal const int INFO_CONTINUE = 100;

        internal const int SUCCESS_OK = 200;
        internal const int SUCCESS_CREATED = 201;
        internal const int SUCCESS_NO_CONTENT = 204;
        internal const int SUCCESS_MULTISTATUS = 207;

        internal const int CLIENT_ERROR_BAD_REQUEST = 400;
        internal const int CLIENT_ERROR_FORBIDDEN = 403;
        internal const int CLIENT_ERROR_NOT_FOUND = 404;
        internal const int CLIENT_ERROR_METHOD_NOT_ALLOWED = 405;
        internal const int CLIENT_ERROR_PRECONDITION_FAILED = 412;
        internal const int CLIENT_ERROR_LOCKED = 423;
        internal const int CLIENT_ERROR_FAILED_DEPENDENCY = 424;

        internal const int SERVER_ERROR_INTERNAL_ERROR = 500;
    }
}
