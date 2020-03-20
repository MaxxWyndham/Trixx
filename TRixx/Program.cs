using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TRixx
{
    class Program
    {
        static readonly int[] header = { 18, 8, 2, 2 };
        static readonly int[] palette = { 16711935, 6095619, 9634306, 13172993, 16711680, 16728128, 16744319, 16760767, 4002056, 7214854, 10362372, 13575170, 16722432, 16736321, 16684930, 16698819, 4332553, 7416327, 10565893, 13649666, 16733440, 16743482, 16687732, 16697774, 5911309, 8603146, 11360519, 14052355, 16744192, 16752189, 16694395, 16702392, 4862728, 7819270, 10841604, 13798146, 16754688, 16760384, 16766081, 16771777, 4865801, 7824647, 10848773, 13807618, 16766208, 16703299, 16640645, 16577736, 5394954, 8224008, 11118853, 13947907, 16776960, 16711244, 16711319, 16645603, 4608526, 6978827, 9349383, 11654148, 14024448, 14745158, 15400331, 16121041, 2966528, 5010176, 7053824, 9031680, 11075328, 12648263, 14220941, 15793876, 5131591, 7367765, 9538404, 11774322, 13944960, 14669212, 15328184, 16052436, 1591296, 2586112, 3580672, 4575488, 5570048, 8257085, 10943867, 13630904, 402944, 2640929, 4813377, 7051362, 9223810, 10736538, 12314802, 13827530, 16917, 28965, 41269, 53316, 65364, 4259711, 8388265, 12582612, 17185, 1925439, 3899228, 5807226, 7715479, 9554605, 11393732, 13232858, 15143, 27719, 40296, 52872, 65448, 4194238, 8323027, 12451817, 10016, 934457, 1793362, 2717802, 3576707, 5810330, 8109490, 10343113, 13878, 26728, 39835, 52685, 65535, 4194046, 8388350, 12516861, 10031, 21347, 32407, 43723, 54783, 4186110, 8382974, 12514301, 9786, 18283, 26525, 35022, 43263, 3980799, 7983615, 11921151, 6712, 12906, 18844, 25037, 30975, 3248895, 6532607, 9750527, 60, 109, 158, 206, 255, 3816191, 7566335, 11382271, 8072704, 9652509, 11232571, 12746840, 14326645, 15120283, 15979712, 16773350, 2230272, 4988432, 7746848, 10439472, 13197632, 13270115, 13342341, 13414824, 5321499, 6503453, 7751199, 8933153, 10115107, 12282666, 14515505, 16683064, 6176801, 7620633, 9064466, 10442506, 11886338, 13468725, 15116904, 16699291, 3745551, 5061411, 6443064, 7758924, 9074784, 10588282, 12102036, 13615534, 2827800, 3946022, 5064244, 6182465, 7300687, 9537649, 11708818, 13945780, 5423, 1189439, 2438991, 3623006, 4807022, 6911883, 8951463, 11056324, 1315860, 2171169, 3026478, 3947324, 4802889, 5658198, 6513507, 7368816, 8289662, 9144971, 10000280, 10855589, 11711154, 12632000, 13487309, 14342618, 0, 1119273, 2237752, 3355976, 4474455, 5592934, 6711413, 7829893, 8948116, 10066595, 11185074, 12303553, 13421777, 14540256, 15658735, 16777215 };
        static readonly string version = "1.0.2";

        static void Main(string[] args)
        {
            string lastCommand = "";
            bool bStillNothing = true;
            TRixxSettings settings = new TRixxSettings();
            List<string> commands = new List<string>();
            string buff;

            if (args.Length > 0)
            {
                foreach (string s in args)
                {
                    if (s.StartsWith("-"))
                    {
                        // functionality
                        switch (s)
                        {
                            case "-iwanttofiddle":
                                lastCommand = "iwanttofiddle";
                                commands.Add("fiddle");
                                break;

                            case "-c1pix":
                                lastCommand = "c1pix";
                                commands.Add("pixbuilder");
                                break;

                            case "-silent":
                                lastCommand = "silent";
                                settings.quiet = true;
                                break;

                            case "-log":
                                lastCommand = "log";
                                settings.quiet = true;
                                break;

                            case "-forcetif":
                                lastCommand = "forcetif";
                                settings.forceTIF = true;
                                break;

                            case "-in":
                                lastCommand = "pathin";
                                break;

                            case "-out":
                                lastCommand = "pathout";
                                break;

                            default:
                                Console.WriteLine("Unknown arguement: " + s);
                                return;
                        }
                    }
                    else
                    {
                        switch (lastCommand)
                        {
                            case "log":
                                settings.logFile = s;
                                break;

                            case "pathin":
                                settings.inDir = s;
                                break;

                            case "pathout":
                                settings.outDir = s;
                                break;
                        }

                        lastCommand = "";
                    }
                }
            }

            if (commands.Count == 0) { commands.Add("pixprocess"); }

            foreach (string command in commands)
            {
                switch (command)
                {
                    case "fiddle":
                        unpackFilesHere(settings.inDir, settings);
                        bStillNothing = false;
                        break;

                    case "pixbuilder":
                        string pixName = settings.inDir.Substring(settings.inDir.Substring(0, settings.inDir.Length - 1).LastIndexOf("\\") + 1).ToUpper().Replace("\\", "").Replace(":", "") + ".PIX";
                        Console.WriteLine("Please enter a filename or press enter to use " + pixName);
                        buff = Console.ReadLine();
                        pixName = (buff.Length > 0 ? buff : pixName);
                        BuildPIXFromFolder(settings, pixName);
                        bStillNothing = false;
                        break;

                    case "pixprocess":
                        foreach (FileInfo fi in getPIXTypeFilesInFolder(new DirectoryInfo(settings.inDir)))
                        {
                            bStillNothing = false;
                            ProcessPIX(fi, settings);
                        }
                        break;
                }
            }

            while (bStillNothing)
            {
                Console.Clear();
                Console.WriteLine("Toxic Ragers presents TRixx for PIX v" + version);
                Console.WriteLine();
                Console.WriteLine("   Options...");
                Console.WriteLine("1) Process folder");
                Console.WriteLine("2) Process everything! (in and below input folder) [-iwanttofiddle]");
                Console.WriteLine("3) Build C1 .PIX file from input folder [-c1pix]");
                Console.WriteLine("4) Create .PAL file from 256 colour .BMP [-newpal]");
                Console.WriteLine();
                Console.WriteLine("   Settings...");
                Console.WriteLine("a) Set input folder (" + tidyPath(settings.inDir) + ")");
                Console.WriteLine("b) Set output folder (" + tidyPath(settings.outDir) + ")");
                Console.WriteLine("c) Toggle output (" + (settings.quiet ? "silent" : "verbose") + ")");
                Console.WriteLine("d) Toggle logging (to " + (settings.toFile ? "file" : "screen") + ")");
                Console.WriteLine("e) Force tiff (" + (settings.forceTIF ? "true" : "false") + ")");
                Console.WriteLine();
                Console.WriteLine("Q) Quit");
                Console.WriteLine();
                Console.WriteLine("Please select an option...");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        foreach (FileInfo fi in getPIXTypeFilesInFolder(new DirectoryInfo(settings.inDir)))
                        {
                            bStillNothing = false;
                            ProcessPIX(fi, settings);
                        }
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        unpackFilesHere(settings.inDir, settings);
                        bStillNothing = false;
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.Clear();
                        string pixName = settings.inDir.Substring(settings.inDir.Substring(0, settings.inDir.Length - 1).LastIndexOf("\\") + 1).ToUpper().Replace("\\", "").Replace(":", "") + ".PIX";
                        Console.WriteLine("Please enter a filename or press enter to use " + pixName);
                        buff = Console.ReadLine();
                        pixName = (buff.Length > 0 ? buff : pixName);
                        bStillNothing = false;

                        BuildPIXFromFolder(settings, pixName);

                        break;

                    case ConsoleKey.A:
                        Console.Clear();
                        Console.WriteLine("Please enter an input folder...");
                        Console.WriteLine();
                        settings.inDir = Console.ReadLine();
                        settings.outDir = settings.inDir;
                        break;

                    case ConsoleKey.B:
                        Console.Clear();
                        Console.WriteLine("Please enter an output folder...");
                        Console.WriteLine();
                        settings.outDir = Console.ReadLine();
                        break;

                    case ConsoleKey.C:
                        settings.quiet = !settings.quiet;
                        break;

                    case ConsoleKey.D:
                        settings.toFile = !settings.toFile;

                        if (settings.toFile)
                        {
                            Console.Clear();
                            Console.WriteLine("Please enter a log name to or press enter to use " + settings.logFile);
                            buff = Console.ReadLine();
                            settings.logFile = (buff.Length > 0 ? buff : settings.logFile);
                        }
                        break;

                    case ConsoleKey.E:
                        settings.forceTIF = !settings.forceTIF;
                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        break;
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static string tidyPath(string path)
        {
            if (path.Length < 35) { return path; }

            return path.Substring(0, 15) + "..." + path.Substring(path.Length - 15);
        }

        private static FileInfo[] getPIXTypeFilesInFolder(DirectoryInfo di)
        {
            ArrayList files = new ArrayList();

            foreach (FileInfo fi in di.GetFiles())
            {
                switch (fi.Extension.ToLower())
                {
                    case ".p08":
                    case ".p16":
                    case ".pix":
                    case ".tab":
                    case ".pal":
                        files.Add(fi);
                        break;
                }
            }

            return (FileInfo[])(files.ToArray(typeof(FileInfo)));
        }

        private static void unpackFilesHere(string path, TRixxSettings settings)
        {
            logLine(path + "\r\n", settings);
            DirectoryInfo here = new DirectoryInfo(path);

            foreach (FileInfo fi in here.GetFiles("*.twt"))
            {
                ProcessTWT(fi);
                fi.MoveTo(fi.FullName.Replace(fi.Extension, ".TWaT"));
            }

            foreach (FileInfo fi in here.GetFiles("pixies.p*"))
            {
                logLine("pixie found: " + fi.Name, settings);
                ProcessPIX(fi, settings, true);
                fi.Delete();
            }

            foreach (FileInfo fi in here.GetFiles("*.pix"))
            {
                logLine("pix file found: " + fi.Name, settings);
                ProcessPIX(fi, settings, false, true);
                //fi.Delete();
            }

            foreach (DirectoryInfo di in here.GetDirectories())
            {
                unpackFilesHere(di.FullName, settings);
            }
        }

        public static void ProcessTWT(FileInfo fi)
        {
            string dest = fi.DirectoryName + "\\" + fi.Name.Replace(fi.Extension, "");
            if (!Directory.Exists(dest)) { Directory.CreateDirectory(dest); }

            BinaryReader br = new BinaryReader(new FileStream(fi.FullName, FileMode.Open));

            int filesize = br.ReadInt32();
            int filecount = br.ReadInt32();

            int[] filesizes = new int[filecount];
            string[] filenames = new string[filecount];

            for (int i = 0; i < filecount; i++)
            {
                filesizes[i] = br.ReadInt32();
                filenames[i] = twtFileName(br.ReadBytes(52));
            }

            for (int i = 0; i < filecount; i++)
            {
                BinaryWriter bw = new BinaryWriter(new FileStream(dest + "\\" + filenames[i], FileMode.Create));
                bw.Write(br.ReadBytes(filesizes[i]));
                bw.Close();

                // Each block is padded to the nearest multiple of 4;
                if ((filesizes[i] % 4) != 0) { br.ReadBytes(4 - (filesizes[i] % 4)); }
            }

            br.Close();
        }

        private static string twtFileName(byte[] t)
        {
            string r = "";

            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == 0) { break; }
                r += (char)t[i];
            }

            return r;
        }

        private static void logLine(string line, TRixxSettings settings)
        {
            if (settings.quiet) { return; }

            if (settings.toFile)
            {
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\" + settings.logFile, true);
                sw.WriteLine(line);
                sw.Close();
            }
            else
            {
                Console.WriteLine(line);
            }
        }

        public static void ProcessPIX(FileInfo fi, TRixxSettings settings,  bool pixie = false, bool massUnpack = false)
        {
            string dest = fi.DirectoryName + (pixie ? (fi.Extension.EndsWith("08") ? "\\tiffx\\" : "\\tiffrgb\\") : "\\");
            if (massUnpack) { dest = fi.Directory.Parent.FullName + (fi.DirectoryName.EndsWith("8") ? "\\tiffx\\" : "\\tiffrgb\\"); }
            if (!Directory.Exists(dest)) { Directory.CreateDirectory(dest); }
            bool c1 = false;

            //Console.WriteLine(fi.Name);

            string filename = fi.Name.Replace(fi.Extension, "");

            BinaryReader br = new BinaryReader(new FileStream(fi.FullName, FileMode.Open));
            Color[] colours = new Color[256];

            for (int i = 0; i < colours.Length; i++)
            {
                colours[i] = R8G8B8ToColour(palette[i]);
            }

            string sout = "";

            for (int i = 0; i < 4; i++)
            {
                if ((int)ReadUInt32(br) != header[i])
                {
                    logLine(fi.Name + " is not a valid PIX file", settings);
                    br.Close();
                    return;
                }
            }

            byte bType = 0;
            int RowSize = 0;
            int Width = 0;
            int Height = 0;

            int lastWidth = 0;
            int lastHeight = 0;
            string lastName = "";
            string lastsout = "";
            byte lastbType = 0;
            int lastRowSize = 0;

            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                if (br.BaseStream.Position + 8 > br.BaseStream.Length)
                {
                    logLine($"Unexpected end of file! ({(br.BaseStream.Length - br.BaseStream.Position)} bytes remaining)!", settings);
                    br.BaseStream.Position = br.BaseStream.Length;
                    continue;
                }

                int block = (int)ReadUInt32(br);
                int blockSize = (int)ReadUInt32(br);
                char s;
                string tmpname;
                bool bUsingRowSize = false;

                switch (block)
                {
                    case 61: // 00 00 00 3D
                        // Standard C2 PIX header
                        sout += blockSize + "\t";
                        bType = br.ReadByte();
                        switch (bType)
                        {
                            case 3:
                                // just a regular 8bit pix
                                break;
                            case 5:
                                // just a regular 16bit pix
                                break;
                            case 18:
                                // 16bit pix with alpha channel
                                break;
                            default:
                                logLine(fi.Name + " :: Unknown type (" + bType + ")", settings);
                                return;
                        }
                        sout += bType + "\t";
                        RowSize = ReadUInt16(br);
                        Width = ReadUInt16(br);
                        Height = ReadUInt16(br);
                        for (int i = 0; i < 6; i++) { if (br.ReadByte() != 0) { logLine("Unknown block contains data", settings); } }

                        s = (char)br.ReadByte();
                        tmpname = "";

                        while (s != 0)
                        {
                            tmpname += s;
                            s = (char)br.ReadByte();
                        }
                        if (tmpname.Length > 0) { filename = tmpname; }

                        sout += RowSize + "\t";
                        sout += Width + "x" + Height + "\t";
                        break;

                    case 3:
                        // Standard C1 pix header and other weird files
                        if (sout != "")
                        {
                            lastsout = sout;
                            sout = "";
                        }
                        
                        sout += br.BaseStream.Position + "\t";
                        sout += blockSize + "\t";
                        lastbType = bType;
                        bType = br.ReadByte();
                        switch (bType)
                        {
                            case 3:
                                // just a regular 8bit pix
                                c1 = true;
                                break;
                            case 6:
                                // 24 bit pix?!
                                break;
                            case 7:
                                // colour palette
                                break;
                            default:
                                logLine(fi.Name + " :: Unknown type (" + bType + ")", settings);
                                return;
                        }
                        sout += bType + "\t";

                        lastRowSize = RowSize;
                        RowSize = ReadUInt16(br); // Width * PixelSize

                        lastWidth = Width;
                        Width = ReadUInt16(br);

                        lastHeight = Height;
                        Height = ReadUInt16(br);

                        int halfWidth = ReadUInt16(br);
                        int halfHeight = ReadUInt16(br);

                        sout += RowSize + "\t";
                        sout += Width + "x" + Height + "\t";
                        sout += halfWidth + "\t";
                        sout += halfHeight + "\t";

                        lastName = filename;
                        s = (char)br.ReadByte();
                        tmpname = "";

                        while (s != 0)
                        {
                            tmpname += s;
                            s = (char)br.ReadByte();
                        }
                        if (tmpname.Length > 0) { filename = tmpname; }

                        break;

                    case 33: // 00 00 00 21
                        // Image data
                        int PixelCount = (int)ReadUInt32(br);
                        if (PixelCount != (Width * Height) || RowSize > Width) { bUsingRowSize = true; }
                        sout += PixelCount + "\t";
                        int PixelSize = (int)ReadUInt32(br);
                        sout += PixelSize + "\t";
                        sout += "RS" + (bUsingRowSize ? ">" : "=") + "W\t";
                        logLine(sout + filename, settings);

                        if (bUsingRowSize) { RowSize /= PixelSize; }

                        if ((c1 || bType == 7) && !settings.forceTIF)
                        {
                            string outPath = Path.Combine(dest, Path.GetFileNameWithoutExtension(fi.Name), $"{filename.ToUpper().Replace(".PIX", "")}{(bType == 7 ? "_pal" : "")}.bmp");

                            Directory.CreateDirectory(Path.GetDirectoryName(outPath));

                            BinaryWriter bmp = new BinaryWriter(new FileStream(outPath, FileMode.Create));

                            bmp.Write(new byte[] { 66, 77 });
                            bmp.Write(14 + 40 + 1024 + (RowSize * Height));
                            bmp.Write((short)0);
                            bmp.Write((short)0);
                            bmp.Write(1078);        // Offset to pixel data

                            // BITMAPINFOHEADER
                            bmp.Write(40);          // Size of header
                            if (bType == 7 && PixelSize == 1)
                            {
                                bmp.Write(16);
                                bmp.Write(16);
                            }
                            else
                            {
                                bmp.Write(Width);
                                bmp.Write(Height);
                            }
                            bmp.Write((short)1);    // Number of planes?  Always 1
                            bmp.Write((short)8);    // Bits per pixel
                            bmp.Write(0);           // No compression
                            bmp.Write(RowSize * Height);           // Raw size
                            bmp.Write(3780);        // Horizontal pixels per metre
                            bmp.Write(3780);        // Vertical pixels per metre
                            bmp.Write(256);         // Number of colours
                            bmp.Write(256);         // Number of important colours

                            if (PixelSize == 4)
                            {
                            }
                            if (bType == 7)
                            {
                                // the input file IS the palette
                                for (int i = 0; i < colours.Length; i++)
                                {
                                    Color c = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());

                                    bmp.Write(c.B);
                                    bmp.Write(c.G);
                                    bmp.Write(c.R);
                                    bmp.Write((byte)0);
                                }

                                for (int y = 15; y >= 0; y--) { for (int x = 0; x < 16; x++) { bmp.Write((byte)((y * 16) + x)); } }
                            }
                            else
                            {
                                // Palette
                                for (int i = 0; i < colours.Length; i++)
                                {
                                    bmp.Write(colours[i].B);
                                    bmp.Write(colours[i].G);
                                    bmp.Write(colours[i].R);
                                    bmp.Write((byte)0);
                                }

                                byte[,] imgdata = new byte[Height, RowSize];

                                for (int y = 0; y < Height; y++) { for (int x = 0; x < RowSize; x++) { imgdata[y, x] = br.ReadByte(); } }
                                for (int y = Height - 1; y >= 0; y--) { for (int x = 0; x < RowSize; x++) { bmp.Write(imgdata[y, x]); } }
                            }

                            bmp.Close();
                        }
                        else
                        {
                            Bitmap bmp = new Bitmap(Width, Height, FormatFromType(bType));

                            for (int y = 0; y < Height; y++)
                            {
                                for (int x = 0; x < RowSize; x++)
                                {
                                    Color c = Color.Black;

                                    switch (bType)
                                    {
                                        case 3:
                                            c = colours[br.ReadByte()];
                                            break;
                                        case 6:
                                            c = Color.FromArgb(0, br.ReadByte(), br.ReadByte(), br.ReadByte());
                                            break;
                                        case 7:
                                            c = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                                            break;
                                        case 5:
                                            c = R5G6B5ToColour(ReadUInt16(br));
                                            break;
                                        case 18:
                                            c = A4R4G4B4ToColour(ReadUInt16(br));
                                            break;
                                    }

                                    if (x < Width) { bmp.SetPixel(x, y, c); }
                                }
                            }

                            if (!Directory.Exists(dest + fi.Name.Replace(fi.Extension, ""))) { Directory.CreateDirectory(dest + fi.Name.Replace(fi.Extension, ""));}
                            bmp.Save(dest + fi.Name.Replace(fi.Extension, "") + "\\" + filename.Replace(".PIX", "").Replace(".pix", "") + ".tif", ImageFormat.Tiff);
                        }
                       
                        break;

                    case 34:
                        // end of palette data, beginning of original pix
                        bType = lastbType;
                        Width = lastWidth;
                        Height = lastHeight;
                        filename = lastName;
                        RowSize = lastRowSize;
                        sout = lastsout;
                        break;

                    case 18:
                        // Mastro malformed pix
                        br.ReadBytes(8);
                        break;

                    case 0:
                        // EOF
                        sout = "";
                        break;

                    default:
                        logLine($"{fi.Name} has an unknown section, {block} ({br.BaseStream.Position}).  Aborting", settings);
                        return;
                }
            }

            br.Close();
        }

        public static void BuildPALFromBMP(TRixxSettings settings, string filename)
        {

        }

        public static void BuildPIXFromFolder(TRixxSettings settings, string filename)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(settings.outDir + "\\" + filename, FileMode.Create));

            for (int i = 0; i < 4; i++)
            {
                WriteInt32(header[i], ref bw);
            }

            foreach (FileInfo fi in new DirectoryInfo(settings.inDir).GetFiles("*.bmp"))
            {
                Bitmap bmp = new Bitmap(fi.FullName);
                List<Color> pal = new List<Color>();

                logLine("Adding " + fi.Name + "... ", settings);

                for (int i = 0; i < bmp.Palette.Entries.Length;i++)
                {
                    pal.Add(bmp.Palette.Entries[i]);
                }

                int Width = bmp.Width;
                int Height = bmp.Height;
                int halfWidth = Convert.ToInt32(Width / 2);
                int halfHeight = Convert.ToInt32(Height / 2);
                int RowSize = Width;

                if (!isEven(Width) || !isEven(halfWidth))
                {
                    RowSize = (halfWidth + (isEven(halfWidth) ? 2 : 1)) * 2;
                }

                // PIX header block
                WriteInt32(3, ref bw);
                WriteInt32(12 + fi.Name.Length, ref bw);
                bw.Write((byte)3);
                WriteInt16(RowSize, ref bw);
                WriteInt16(Width, ref bw);
                WriteInt16(Height, ref bw);
                WriteInt16(halfWidth, ref bw);
                WriteInt16(halfHeight, ref bw);
                bw.Write(fi.Name.ToUpper().Replace(".BMP", ".PIX").ToCharArray());
                bw.Write((byte)0);

                // PIX data block
                WriteInt32(33, ref bw);
                WriteInt32(8 + (RowSize*Height), ref bw);
                WriteInt32(RowSize * Height, ref bw);
                WriteInt32(1, ref bw);

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < RowSize; x++)    
                    {
                        if (x < bmp.Width)
                        {
                            bw.Write((byte)pal.IndexOf(bmp.GetPixel(x, y)));
                        }
                        else
                        {
                            bw.Write((byte)0);
                        }
                    }
                }

                // PIX footer block
                WriteInt32(0, ref bw);
                WriteInt32(0, ref bw);

                logLine("DONE", settings);

                bmp = null;
            }

            logLine("Saving " + filename + "... DONE\r\n", settings);

            bw.Close();
        }

        private static bool isEven(int i)
        {
            return (i % 2 == 0);
        }

        public static PixelFormat FormatFromType(byte type)
        {
            switch (type)
            {
                //case 3:
                //    return PixelFormat.Format8bppIndexed;

                case 18:
                    return PixelFormat.Format32bppArgb;

                default:
                    return PixelFormat.Format16bppRgb565;
            }
        }

        public static ushort ReadUInt16(BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(2);
            return (ushort)(bytes[0] << 8 | bytes[1]);
        }

        public static uint ReadUInt32(BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(4);
            uint[] ints = new uint[4];

            ints[0] = bytes[0];
            ints[1] = bytes[1];
            ints[2] = bytes[2];
            ints[3] = bytes[3];

            ints[0] <<= 24;
            ints[1] <<= 16;
            ints[2] <<= 8;
            ints[3] <<= 0;

            return (ints[0] | ints[1] | ints[2] | ints[3]);
        }

        public static void WriteInt16(int i, ref BinaryWriter bw)
        {
            byte[] b = BitConverter.GetBytes((short)i);
            Array.Reverse(b);
            bw.Write(BitConverter.ToInt16(b, 0));
        }

        public static void WriteInt32(int i, ref BinaryWriter bw)
        {
            byte[] b = BitConverter.GetBytes(i);
            Array.Reverse(b);
            bw.Write(BitConverter.ToInt32(b, 0));
        }

        public static Color EightBitToColour(int i)
        {
            int r = (i & 0xF800) << 8;
            int g = (i & 0x7E0) << 5;
            int b = (i & 0x1F) << 3;

            return Color.FromArgb(r | g | b);
        }

        public static Color R5G6B5ToColour(int i)
        {
            int r = (i & 0xF800) << 8;
            int g = (i & 0x7E0) << 5;
            int b = (i & 0x1F) << 3;

            return Color.FromArgb(r | g | b);
        }

        public static Color A4R4G4B4ToColour(int i)
        {
            int a = (i & 0xF000) >> 12;
            int r = (i & 0xF00) >> 8;
            int g = (i & 0xF0) >> 4;
            int b = (i & 0xF);

            return Color.FromArgb(a * 17, r * 17, g * 17, b * 17);
        }

        public static Color R8G8B8ToColour(int i)
        {
            int a = 0;
            int r = (i & 0xFF0000) >> 16;
            int g = (i & 0xFF00) >> 8;
            int b = (i & 0xFF);

            return Color.FromArgb(a, r, g, b);
        }
    }

    public class TRixxSettings
    {
        public bool quiet;
        public bool toFile;
        public bool forceTIF;
        public string logFile;
        public string inDir;
        public string outDir;

        public TRixxSettings()
        {
            quiet = false;
            toFile = false;
            forceTIF = false;
            logFile = "TRixx.log";

            inDir = Directory.GetCurrentDirectory();
            outDir = Directory.GetCurrentDirectory();
        }
    }
}
