using System;

namespace DeleteDuplicateFiles
{
    internal class Summary
    {
        private bool _isSimulation;
        private int _filesDeleted;
        private long _bytesFreed;

        public Summary(bool isSimulation)
        {
            _isSimulation = isSimulation;
        }

        public void RegisterFileDeletion(long bytesFreed)
        {
            ++_filesDeleted;
            _bytesFreed += bytesFreed;
        }

        public void Show()
        {
            Console.WriteLine();

            if (_isSimulation)
            {
                if (_filesDeleted > 0)
                {
                    Console.WriteLine($"{_filesDeleted} files would have been deleted and {_bytesFreed} bytes would have been recovered.");
                }
                else
                {
                    Console.WriteLine($"No files would have been deleted.");
                }
            }
            else
            {
                if (_filesDeleted > 0)
                {
                    Console.WriteLine($"{_filesDeleted} files have been deleted and {_bytesFreed} bytes have been recovered.");
                }
                else
                {
                    Console.WriteLine($"No files have been deleted.");
                }
            }
        }
    }
}
