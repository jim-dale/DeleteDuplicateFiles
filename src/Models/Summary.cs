namespace DeleteDuplicateFiles.Models;

using System;

internal class Summary
{
    private readonly bool isSimulation;
    private int filesDeleted;
    private long bytesFreed;

    public Summary(bool isSimulation)
    {
        this.isSimulation = isSimulation;
    }

    public void RegisterFileDeletion(long bytesFreed)
    {
        ++this.filesDeleted;
        this.bytesFreed += bytesFreed;
    }

    public void Show()
    {
        Console.WriteLine();

        if (this.isSimulation)
        {
            if (this.filesDeleted > 0)
            {
                Console.WriteLine($"{this.filesDeleted} files would have been deleted and {this.bytesFreed} bytes would have been recovered.");
            }
            else
            {
                Console.WriteLine($"No files would have been deleted.");
            }
        }
        else
        {
            if (this.filesDeleted > 0)
            {
                Console.WriteLine($"{this.filesDeleted} files have been deleted and {this.bytesFreed} bytes have been recovered.");
            }
            else
            {
                Console.WriteLine($"No files have been deleted.");
            }
        }
    }
}
