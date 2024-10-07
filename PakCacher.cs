using System;
using System.IO;
using System.Linq;

namespace pakcacher
{
    internal class PakCacher
    {
        struct ZipFileInfo
        {
            public int signature;
            public short version;
            public short flag;
            public short compressmethod;
            public short modtime;
            public short moddate;
            public int crc;
            public int compressed;
            public int uncompressed;
            public short filenamelen;
            public short extrafieldlen;
            public char[] filename;
            public int offset;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No file provided. Drag and drop *.pak or *.zip file(s) in the executable");
                Console.WriteLine("Press any key to close window...");
                Console.ReadKey();
                return;
            }

            int ctr = 1;
            foreach (string filepath in args)
            {
                Console.WriteLine("Processing file {0} of {1}", ctr, args.Length);
                if (File.Exists(filepath)
                    & (".pak".Equals(Path.GetExtension(filepath)) | ".zip".Equals(Path.GetExtension(filepath))
                    )
                )
                {
                    Console.WriteLine("Input: " + filepath);
                    try
                    {
                        bool isPak = ".pak".Equals(Path.GetExtension(filepath));

                        CheckSignature(filepath);

                        using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                        {
                            string outputfilename = (isPak) ? fileStream.Name + ".cache" : fileStream.Name.Replace(Path.GetExtension(filepath), "") + ".header";

                            Console.WriteLine("Output: " + outputfilename);

                            if (File.Exists(outputfilename))
                                File.Move(outputfilename, outputfilename + "." + DateTime.Now.ToString("yyyyMMddHHmmssffff"));


                            using (BinaryReader reader = new BinaryReader(fileStream))
                            using (FileStream writestream = new FileStream(outputfilename, FileMode.Create, FileAccess.ReadWrite))
                            {
                                if (!isPak)
                                {
                                    writestream.Write(new byte[] { 0x51, 0x34, 0x00, 0x00 }, 0, 4);
                                }

                                ZipFileInfo zipFileInfo;

                                zipFileInfo.signature = reader.ReadInt32();
                                zipFileInfo.offset = 0;

                                while (zipFileInfo.signature.Equals(67324752))
                                {

                                    zipFileInfo.version = reader.ReadInt16();
                                    zipFileInfo.flag = reader.ReadInt16();
                                    zipFileInfo.compressmethod = reader.ReadInt16();
                                    zipFileInfo.modtime = reader.ReadInt16();
                                    zipFileInfo.moddate = reader.ReadInt16();
                                    zipFileInfo.crc = reader.ReadInt32();
                                    zipFileInfo.compressed = reader.ReadInt32();
                                    zipFileInfo.uncompressed = reader.ReadInt32();
                                    zipFileInfo.filenamelen = reader.ReadInt16();
                                    zipFileInfo.extrafieldlen = reader.ReadInt16();
                                    zipFileInfo.filename = reader.ReadChars(zipFileInfo.filenamelen);

                                    // to data offset
                                    zipFileInfo.offset += 30 + zipFileInfo.filenamelen;

                                    //skip file data
                                    reader.ReadBytes(zipFileInfo.compressed);

                                    if (isPak) CreateCache(zipFileInfo, writestream);
                                    else CreateHeader(zipFileInfo, writestream);

                                    //next file offset
                                    zipFileInfo.offset += zipFileInfo.compressed;

                                    // advance to next file sig
                                    zipFileInfo.signature = reader.ReadInt32();
                                }
                            }

                            Console.WriteLine("File {0}: {1} completed\n", ctr, outputfilename);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString() + "\n");
                    }

                }
                else
                {
                    Console.WriteLine("{0} is not a *.pak or a *.zip\n", Path.GetFileName(filepath));
                }
                ctr++;
            }

            Console.WriteLine("Completed processing {0} file(s)", args.Length);
            Console.WriteLine("Press any key to close window...");
            Console.ReadKey();
        }

        private static void CheckSignature(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                if (!reader.ReadInt32().Equals(67324752))
                    throw new Exception("The file " + filePath + " header did not start with the expected signature. Make sure the file is correct.");
            }
        }

        private static void CreateCache(ZipFileInfo zipFileInfo, FileStream writestream)
        {
            byte[] toWrite = BitConverter.GetBytes((Int32)6)
                .Concat(new byte[] { 0x3F })
                .Concat(BitConverter.GetBytes((Int32)zipFileInfo.filenamelen))
                .Concat(System.Text.Encoding.UTF8.GetBytes(zipFileInfo.filename))
                .Concat(BitConverter.GetBytes((Int64)zipFileInfo.offset))
                .Concat(BitConverter.GetBytes((Int64)zipFileInfo.uncompressed))
                .Concat(BitConverter.GetBytes((Int64)zipFileInfo.compressed))
                .Concat(BitConverter.GetBytes(zipFileInfo.compressmethod))
                .Concat(BitConverter.GetBytes(zipFileInfo.crc))
                .ToArray();
            writestream.Write(toWrite, 0, toWrite.Length);
        }

        private static void CreateHeader(ZipFileInfo zipFileInfo, FileStream writestream)
        {

            byte[] toWrite = BitConverter.GetBytes((Int32)zipFileInfo.filenamelen)
                .Concat(System.Text.Encoding.UTF8.GetBytes(zipFileInfo.filename))
                .Concat(BitConverter.GetBytes((Int64)zipFileInfo.offset))
                .Concat(BitConverter.GetBytes((Int64)zipFileInfo.uncompressed))
                .ToArray();
            writestream.Write(toWrite, 0, toWrite.Length);

        }
    }
}
