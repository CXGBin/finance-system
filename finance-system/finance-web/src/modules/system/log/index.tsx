import React, { useRef } from 'react';
import { Tag, Drawer, Descriptions } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import type { OperLog } from '@/types/system.d';
import { logApi } from '@/api/system';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 操作日志页面（只读） */
const LogList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [drawerOpen, setDrawerOpen] = React.useState(false);
  const [currentLog, setCurrentLog] = React.useState<OperLog | null>(null);

  const columns: ProColumns<OperLog>[] = [
    {
      title: '操作模块',
      dataIndex: 'module',
      ellipsis: true,
    },
    {
      title: '操作类型',
      dataIndex: 'action',
      ellipsis: true,
    },
    {
      title: '操作人',
      dataIndex: 'userName',
      ellipsis: true,
    },
    {
      title: '操作IP',
      dataIndex: 'ipAddress',
      search: false,
      width: 130,
    },
    {
      title: '请求方法',
      dataIndex: 'requestMethod',
      search: false,
      width: 90,
    },
    {
      title: '请求路径',
      dataIndex: 'requestUrl',
      ellipsis: true,
      search: false,
    },
    {
      title: '耗时(ms)',
      dataIndex: 'durationMs',
      sorter: true,
      search: false,
      width: 100,
      align: 'right',
    },
    {
      title: '状态码',
      dataIndex: 'responseCode',
      sorter: true,
      search: false,
      width: 90,
      render: (_, record) => {
        const code = record.responseCode ?? 0;
        return code >= 200 && code < 300 ? <Tag color="success">成功</Tag> : <Tag color="error">失败({code})</Tag>;
      },
    },
    {
      title: '操作时间',
      dataIndex: 'createdTime',
      valueType: 'dateTime',
      sorter: true,
      search: false,
      width: 180,
    },
    {
      title: '操作',
      valueType: 'option',
      width: 60,
      render: (_, record) => (
        <a onClick={() => { setCurrentLog(record); setDrawerOpen(true); }}>详情</a>
      ),
    },
  ];

  return (
    <>
      <ProTable<OperLog>
        actionRef={actionRef}
        headerTitle="操作日志"
        rowKey="id"
        columns={columns}
        search={{ labelWidth: 'auto' }}
        request={createProTableRequest((params) => logApi.list(params as any))}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Drawer title="日志详情" open={drawerOpen} onClose={() => setDrawerOpen(false)} width={600}>
        {currentLog && (
          <Descriptions column={1} bordered size="small">
            <Descriptions.Item label="操作模块">{currentLog.module}</Descriptions.Item>
            <Descriptions.Item label="操作类型">{currentLog.action}</Descriptions.Item>
            <Descriptions.Item label="操作人">{currentLog.userName}</Descriptions.Item>
            <Descriptions.Item label="操作IP">{currentLog.ipAddress}</Descriptions.Item>
            <Descriptions.Item label="请求方法">{currentLog.requestMethod}</Descriptions.Item>
            <Descriptions.Item label="请求路径">{currentLog.requestUrl}</Descriptions.Item>
            <Descriptions.Item label="耗时">{currentLog.durationMs}ms</Descriptions.Item>
            <Descriptions.Item label="状态码">{currentLog.responseCode}</Descriptions.Item>
            <Descriptions.Item label="操作时间">{currentLog.createdTime}</Descriptions.Item>
            <Descriptions.Item label="描述">{currentLog.description}</Descriptions.Item>
            <Descriptions.Item label="请求体"><pre style={{ maxHeight: 200, overflow: 'auto' }}>{currentLog.requestBody}</pre></Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
};

export default LogList;
