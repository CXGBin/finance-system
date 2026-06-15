import React from 'react';
import { Tag, Space, Button } from 'antd';
import ProTable from '@/components/ProTable';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';
import type { ApprovalInstance } from '@/types/approval.d';

/** 待办审批页面 */
const PendingApproval: React.FC = () => {
  const navigate = useNavigate();
  const columns = [
    { title: '审批单号', dataIndex: 'approvalNo', key: 'approvalNo', search: true },
    { title: '标题', dataIndex: 'title', key: 'title' },
    { title: '发起人', dataIndex: 'initiatorName', key: 'initiatorName' },
    { title: '发起时间', dataIndex: 'createdTime', key: 'createdTime' },
    {
      title: '操作', key: 'action', render: (_: any, record: ApprovalInstance) => (
        <Space>
          <a onClick={() => navigate(`/approval/${record.id}`)}>详情</a>
          <a onClick={() => approvalApi.approve(record.id, '同意').then(() => window.location.reload())}>通过</a>
          <a style={{ color: '#ff4d4f' }} onClick={() => approvalApi.reject(record.id, '驳回').then(() => window.location.reload())}>驳回</a>
        </Space>
      ),
    },
  ];
  return <ProTable columns={columns} fetchData={(params) => approvalApi.pending(params as any)} />;
};

export default PendingApproval;
