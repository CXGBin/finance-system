import React from 'react';
import ProTable from '@/components/ProTable';
import { logApi } from '@/api/system';
import { Tag } from 'antd';

/** 操作日志页面（只读） */
const LogList: React.FC = () => {
  const columns = [
    { title: '操作模块', dataIndex: 'module', key: 'module', search: true },
    { title: '操作类型', dataIndex: 'action', key: 'action' },
    { title: '操作人', dataIndex: 'userName', key: 'userName', search: true },
    { title: '操作IP', dataIndex: 'ipAddress', key: 'ipAddress' },
    { title: '请求路径', dataIndex: 'requestUrl', key: 'requestUrl' },
    { title: '耗时(ms)', dataIndex: 'durationMs', key: 'durationMs', sorter: true },
    { title: '状态码', dataIndex: 'responseCode', key: 'responseCode', render: (val: number) => val >= 200 && val < 300 ? <Tag color="success">成功</Tag> : <Tag color="error">失败</Tag> },
    { title: '操作时间', dataIndex: 'createdTime', key: 'createdTime', sorter: true },
  ];

  return (
    <div>
      <ProTable
        columns={columns}
        fetchData={(params) => logApi.list(params as any)}
      />
    </div>
  );
};

export default LogList;
