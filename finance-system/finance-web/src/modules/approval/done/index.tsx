import React from 'react';
import { Tag } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';

/** 已办审批页面 */
const DoneApproval: React.FC = () => {
  const navigate = useNavigate();
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '业务ID', dataIndex: 'businessId', search: true, sorter: true },
    { title: '标题', dataIndex: 'title', search: true },
    { title: '发起人ID', dataIndex: 'initiatorId', search: true },
    { title: '结果', dataIndex: 'result', valueType: 'select', valueEnum: { 1: { text: '通过' }, 2: { text: '驳回' } }, search: true },
    { title: '处理时间', dataIndex: 'handleTime', sorter: true },
    {
      title: '操作', key: 'action', search: false,
      render: (_, record) => <a onClick={() => navigate(`/approval/${(record as any).id}`)}>查看</a>,
    },
  ];
  const request = createProTableRequest((params) => approvalApi.done(params));
  return <ProTable columns={columns} request={request} search={{ labelWidth: 'auto', defaultCollapsed: true }} rowKey="id" />;
};

export default DoneApproval;
