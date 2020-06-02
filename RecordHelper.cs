﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using b2xtranslator.Spreadsheet.XlsFileFormat;
using b2xtranslator.Spreadsheet.XlsFileFormat.Records;
using b2xtranslator.StructuredStorage.Reader;
using b2xtranslator.Tools;

namespace Macrome
{
    public class RecordHelper
    {
        public static List<BiffRecord> ConvertToSpecificRecords(List<BiffRecord> generalRecords)
        {
            List<BiffRecord> specificRecords = new List<BiffRecord>();
            foreach (var record in generalRecords)
            {
                switch (record.Id)
                {
                    case RecordType.Formula:
                        specificRecords.Add(record.AsRecordType<Formula>());
                        break;
                    case RecordType.Lbl:
                        specificRecords.Add(record.AsRecordType<Lbl>());
                        break;
                    case RecordType.BoundSheet8:
                        specificRecords.Add(record.AsRecordType<BoundSheet8>());
                        break;
                    case RecordType.SupBook:
                        specificRecords.Add(record.AsRecordType<SupBook>());
                        break;
                    case RecordType.ExternSheet:
                        specificRecords.Add(record.AsRecordType<ExternSheet>());
                        break;
                    default:
                        specificRecords.Add(record);
                        break;
                }
            }

            return specificRecords;
        }

        public static List<BiffRecord> ParseBiffStreamBytes(byte[] bytes)
        {
            List<BiffRecord> records = new List<BiffRecord>();
            MemoryStream ms = new MemoryStream(bytes);
            VirtualStreamReader vsr = new VirtualStreamReader(ms);

            while (vsr.BaseStream.Position < vsr.BaseStream.Length)
            {
                RecordType id = (RecordType)vsr.ReadUInt16();
                UInt16 length = vsr.ReadUInt16();

                BiffRecord br = new BiffRecord(vsr, id, length);

                vsr.ReadBytes(length);
                records.Add(br);
            }

            return records;
        }

        public static byte[] ConvertBiffRecordsToBytes(List<BiffRecord> records)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            foreach (var record in records)
            {
                bw.Write(record.GetBytes());
            }
            return bw.GetBytesWritten();
        }

    }
}
