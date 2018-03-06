﻿using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Response;
using Aliyun.Api.LOG.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aliyun.Api.SLS.Response
{
    public class BatchGetLogsResponse : LogResponse
    {
        private string _nextCursor;
        private int _logCount;
        private int _rawSize;
        private LogGroupList _logGroupList;

        public BatchGetLogsResponse(IDictionary<string, string> headers, Stream body)
            : base(headers)
        {
            headers.TryGetValue(LogConsts.NAME_HEADER_NEXT_CURSOR, out _nextCursor);
            if (headers.TryGetValue(LogConsts.NAME_HEADER_LOG_COUNT, out string tmpLogCount))
            {
                int.TryParse(tmpLogCount, out _logCount);
            }
            if (headers.TryGetValue(LogConsts.NAME_HEADER_LOG_BODY_RAW_SIZE, out string tmpRawSize))
            {
                int.TryParse(tmpRawSize, out _rawSize);
            }
            int contentLength = 0;
            if (headers.TryGetValue("Content-Length", out string tmpContentLength))
            {
                int.TryParse(tmpContentLength, out contentLength);
            }
            //_logGroupList = LogGroupList.ParseFrom(LogClientTools.DecompressFromLZ4(body, _rawSize));
            _logGroupList = new LogGroupList();
        }

        public string NextCursor
        {
            get { return _nextCursor; }
        }
        public int LogCount
        {
            get { return _logCount; }
        }
        public int RawSize
        {
            get { return _rawSize; }
        }
        public LogGroupList LogGroupList
        {
            get { return _logGroupList; }
        }
    }
}
