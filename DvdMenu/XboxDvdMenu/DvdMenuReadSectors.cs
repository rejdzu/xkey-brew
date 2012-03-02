using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace IsoGameInfo
{
public class DvdMenuReadSectors
{
	private BinaryReader br;

	private FileStream fs;

	private int pathOffset;

	private int sectorSize;

	public DvdMenuReadSectors(string path)
	{
		this.sectorSize = 2048;
		this.pathOffset = 132;
		try
		{
			this.fs = new FileStream(path, FileMode.Open, FileAccess.Read);
			this.br = new BinaryReader(this.fs);
		}
		catch (Exception exception)
		{
			Exception ex = exception;
			throw new Exception("Error opening iso file", ex);
		}
	}

	private Dictionary<string, byte[]> GetFilesFromVideoTS(int videoTSLBA)
	{
		Dictionary<string, byte[]> videoTSStructure = new Dictionary<string, byte[]>();
		try
		{
			long videoTSSector = (long)this.sectorSize * (long)videoTSLBA;
			this.br.BaseStream.Seek(videoTSSector, SeekOrigin.Begin);
			while (this.br.BaseStream.Position < videoTSSector + (long)(this.sectorSize * 2))
			{
				byte lengthDirectoryRecord = this.br.ReadByte();
				if (lengthDirectoryRecord <= 0)
				{
					continue;
				}
				this.br.ReadByte();
				byte[] sector = this.br.ReadBytes(4);
				BitConverter.ToInt32(sector, 0);
				this.br.BaseStream.Seek((long)4, SeekOrigin.Current);
				this.br.ReadInt32();
				this.br.BaseStream.Seek((long)4, SeekOrigin.Current);
				Encoding.ASCII.GetString(this.br.ReadBytes(7));
				this.br.ReadByte();
				this.br.ReadByte();
				this.br.ReadByte();
				this.br.ReadInt16();
				this.br.BaseStream.Seek((long)2, SeekOrigin.Current);
				byte lenghtOfFileName = this.br.ReadByte();
				byte[] tmp = this.br.ReadBytes(lenghtOfFileName);
				string fileName = Encoding.ASCII.GetString(tmp);
				if (lenghtOfFileName != 1 || tmp[0] != 0)
				{
					if (lenghtOfFileName == 1 && tmp[0] == 1)
					{
						fileName = "..";
					}
				}
				else
				{
					fileName = ".";
				}
				if (lenghtOfFileName % 2 == 0)
				{
					this.br.BaseStream.Seek((long)1, SeekOrigin.Current);
				}
                
				videoTSStructure.Add(fileName.Replace(";1", ""), sector);
			}
		}
		catch (Exception exception)
		{
			Exception ex = exception;
			throw new Exception("Error listing VIDEO_TS", ex);
		}
		return videoTSStructure;
	}

	public Dictionary<string, byte[]> GetFilesWithSectors()
	{
		return this.GetFilesWithSectors(false);
	}

	public Dictionary<string, byte[]> GetFilesWithSectors(bool showLog)
	{
		Dictionary<string, byte[]> videoTSStructure = null;
		int videoTSLba = this.GetLBAOfVideoTS();
		if (videoTSLba > 0)
		{
			videoTSStructure = this.GetFilesFromVideoTS(videoTSLba);
			if (showLog)
			{
				Form fm = new Form();
				fm.Size = new Size(0x320, 0x258);
				TextBox tb = new TextBox();
				tb.Multiline = true;
				tb.Dock = DockStyle.Fill;
				tb.ReadOnly = true;
				tb.Text = this.PrepareLog(videoTSStructure);
				tb.Font = new Font("Courier New", 9f);
				fm.Controls.Add(tb);
				fm.Show();
			}
		}
		return videoTSStructure;
	}

	private int GetLBAOfVideoTS()
	{
		int videoTSLBA = 0;
		try
		{
			this.br.BaseStream.Seek((long)(16 * this.sectorSize + this.pathOffset), SeekOrigin.Begin);
			int pathTableSize = this.br.ReadInt32();
			this.br.BaseStream.Seek((long)4, SeekOrigin.Current);
			int pathTableLittleEndian = this.br.ReadInt32();
			long pathSector = (long)this.sectorSize * (long)pathTableLittleEndian;
			this.br.BaseStream.Seek(pathSector, SeekOrigin.Begin);
			while (this.br.BaseStream.Position < pathSector + (long)pathTableSize)
			{
				byte dirNameLength = this.br.ReadByte();
				this.br.BaseStream.Seek((long)1, SeekOrigin.Current);
				int lba = this.br.ReadInt32();
				this.br.ReadInt16();
				string dirName = Encoding.ASCII.GetString(this.br.ReadBytes(dirNameLength));
				if (dirNameLength % 2 == 1)
				{
					this.br.BaseStream.Seek((long)1, SeekOrigin.Current);
				}
				if (dirName.ToUpper() != "VIDEO_TS")
				{
					continue;
				}
				videoTSLBA = lba;
				break;
			}
		}
		catch (Exception exception)
		{
			Exception ex = exception;
			throw new Exception("Error searching for VIDEO_TS", ex);
		}
		return videoTSLBA;
	}

	private string PrepareLog(Dictionary<string, byte[]> input)
	{
		string log = "";
		log = string.Concat(log, "File name".PadRight(20));
		log = string.Concat(log, "Sector".PadRight(20));
		log = string.Concat(log, "\r\n");
		foreach (KeyValuePair<string, byte[]> kvp in input)
		{
			log = string.Concat(log, kvp.Key.PadRight(20));
			int num = BitConverter.ToInt32(kvp.Value, 0);
			log = string.Concat(log, num.ToString().PadRight(20));
			log = string.Concat(log, "\r\n");
		}
		return log;
	}
}
}