using System;

namespace WebDAVServer.api.helpers {
    class ProgressView {

        private const int DefaultSize = 20;
        private readonly int _mSize;
        private bool _isDrawedProgress;
        private int _count;

        public ProgressView(int size = DefaultSize) {
            _mSize = size;
        }

        public void DrawProgress(double progress) {
            if (!_isDrawedProgress) {
                Console.Out.Write("|");
                for (var i = 0; i < _mSize - 2; i++) {
                    Console.Out.Write(" ");
                }
                Console.Out.WriteLine("|");
                _isDrawedProgress = true;
            }
            if (_count >= (int) (progress*_mSize)) {
                return;
            }
            for (var i = 0; i < (int)(progress * _mSize) - _count; i++) {
                Console.Out.Write(".");
                _count++;
            }
        }
    }
}
