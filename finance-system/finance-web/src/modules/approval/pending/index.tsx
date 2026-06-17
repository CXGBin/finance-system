import React, { useRef } from 'react';
import { Space } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';

/** 待办审批页面 */
const PendingApproval: React.FC = () => {
  const navigate = useNavigate();
  const actionRef = useRef<ActionType>();
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '业务ID', dataIndex: 'businessId', search: true, sorter: true },
    { title: '标题', dataIndex: 'title', search: true },
    { title: '发起人ID', dataIndex: 'initiatorId', search: true },
    { title: '发起时间', dataIndex: 'createdTime', sorter: true },
    {
      title: '操作', key: 'action', search: false,
      render: (_, record) => {
        const r = record as any;
        return (
          <Space>
            <a onClick={() => navigate(`/approval/${r.id}`)}>详情</a>
            <a onClick={() => { approvalApi.approve(r.id, '同意'); setTimeout(() => actionRef.current?.reload(), 500); }}>通过</a>
            <a style={{ color: '#ff4d4f' }} onClick={() => { approvalApi.reject(r.id, '驳回'); setTimeout(() => actionRef.current?.reload(), 500); }}>驳回</a>
          </Space>
        );
      },
    },
  ];
  const request = createProTableRequest((params) => approvalApi.pending(params));
  return <ProTable actionRef={actionRef} columns={columns} request={request} search={{ labelWidth: 'auto' }} rowKey="id" />;
};

export default PendingApproval;
