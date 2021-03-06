﻿// azure-rtable ver. 0.9
//
// Copyright (c) Microsoft Corporation
//
// All rights reserved. 
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
// (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace Microsoft.Azure.Toolkit.Replication.Test
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Exactly the same as SampleRTableEntity, except this class is an "XStore Table Entity"
    /// </summary>
    public sealed class SampleXStoreEntity : TableEntity
    {
        /// <summary>
        /// PartitionKey = JobType.ToLower().Replace(" ", "") 
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// RowKey = JobId.ToLower().Replace(" ", "");
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// This column will store some string value. Change the value of this column in the unit test.
        /// </summary>
        public string Message { get; set; }

        public SampleXStoreEntity()
        {
        }

        public SampleXStoreEntity(string jobType, string jobId, string message)
        {
            string partitionKey;
            string rowKey;
            if (string.IsNullOrEmpty(jobType))
            {
                jobType = "EmptyJobType";
            }
            if (string.IsNullOrEmpty(jobId))
            {
                jobId = "EmptyJobId";
            }

            GenerateKeys(jobType, jobId, out partitionKey, out rowKey);

            PartitionKey = partitionKey;
            RowKey = rowKey;

            JobType = jobType;
            JobId = jobId;
            Message = message;
        }

        /// <summary>
        /// Copy the values of JobType, JobId, Message to the specified object
        /// </summary>
        /// <param name="dst"></param>
        public void CopyTo(SampleRTableEntity dst)
        {
            dst.JobType = this.JobType;
            dst.JobId = this.JobId;
            dst.Message = this.Message;
        }

        /// <summary>
        /// Check whether this instance matches the specified object or not
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            SampleXStoreEntity dst = obj as SampleXStoreEntity;
            if (dst == null)
            {
                return false;
            }
            else
            {
                return (this.JobType == dst.JobType && this.JobId == dst.JobId && this.Message == dst.Message);
            }
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("\tJobType={0}\n\tJobId={1}\n\tMessage={2}\n\tETag={3}",
                JobType,
                JobId,
                Message,
                ETag);
        }

        /// <summary>
        /// Helper function to find out the PartitionKey and RowKey
        /// </summary>
        /// <param name="jobType"></param>
        /// <param name="jobId"></param>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        public static void GenerateKeys(string jobType, string jobId, out string partitionKey, out string rowKey)
        {
            partitionKey = jobType.ToLower().Replace(" ", "");
            rowKey = jobId.ToLower().Replace(" ", "");
        }

        /// <summary>
        /// Helper function to generate a timestamp and a random Guid. Caller can set "Message" to this random string for testing.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomMessage()
        {
            return string.Format("{0:MM/dd/yyyy HH:mm:ss.fff} {1}", DateTime.UtcNow, Guid.NewGuid());
        }

        public static SampleXStoreEntity ToSampleXStoreEntity(DynamicReplicatedTableEntity dynamicReplicatedTableEntity)
        {
            var entity = new SampleXStoreEntity()
            {
                PartitionKey = dynamicReplicatedTableEntity.PartitionKey,
                RowKey = dynamicReplicatedTableEntity.RowKey,
                Timestamp = dynamicReplicatedTableEntity.Timestamp,
                ETag = dynamicReplicatedTableEntity.ETag
            };

            // we could reflect, but keeping it simple.
            entity.JobType = dynamicReplicatedTableEntity.Properties["JobType"].StringValue;
            entity.JobId = dynamicReplicatedTableEntity.Properties["JobId"].StringValue;
            entity.Message = dynamicReplicatedTableEntity.Properties["Message"].StringValue;

            return entity;
        }

        public static InitDynamicReplicatedTableEntity ToInitDynamicReplicatedTableEntity(SampleXStoreEntity xstoreEntity)
        {
            IDictionary<string, EntityProperty> properties = new Dictionary<string, EntityProperty>();

            // we could reflect, but keeping it simple.
            properties.Add("JobType", new EntityProperty(xstoreEntity.JobType));
            properties.Add("JobId", new EntityProperty(xstoreEntity.JobId));
            properties.Add("Message", new EntityProperty(xstoreEntity.Message));

            InitDynamicReplicatedTableEntity entity = new InitDynamicReplicatedTableEntity(
                                                                xstoreEntity.PartitionKey,
                                                                xstoreEntity.RowKey,
                                                                xstoreEntity.ETag,
                                                                properties);
            return entity;
        }
    }
}
