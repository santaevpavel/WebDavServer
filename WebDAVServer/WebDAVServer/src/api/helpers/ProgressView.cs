﻿using System;

namespace WebDAVServer.api.helpers {
    class ProgressView {

        private readonly int mSize;
        private bool isDrawedProgress;
        private int count;

        public ProgressView(int size = 20) {
            mSize = size;
        }

        public void drawProgress(double progress) {
            if (!isDrawedProgress) {
                Console.Out.Write("|");
                for (var i = 0; i < mSize - 2; i++) {
                    Console.Out.Write(" ");
                }
                Console.Out.WriteLine("|");
                isDrawedProgress = true;
            }
            if (count >= (int) (progress*mSize)) {
                return;
            }
            for (var i = 0; i < (int)(progress * mSize) - count; i++) {
                Console.Out.Write(".");
                count++;
            }
        }
    }
}