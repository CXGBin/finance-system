import React from 'react';
import { Tag } from 'antd';
import ProTable from '@/components/ProTable';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';
import type { ApprovalInstance } from '@/types/approval.d';

/** 已办审批页面 */
const DoneApproval: React.FC = () => {
  const navigate = useNavigate();
  const columns = [
    { title: '审批单号', dataIndex: 'approvalNo', key: 'approvalNo', search: true },
    { title: '标题', dataIndex: 'title', key: 'title' },
    { title: '发起人', dataIndex: 'initiatorName', key: 'initiatorName' },
    {
      title: '结果', dataIndex: 'result', key: 'result',
      render: (val: number) => val === 1 ? <Tag color="success">通过</Tag> : <Tag color="error">驳回</Tag>,
    },
    { title: '处理时间', dataIndex: 'handleTime', key: 'handleTime' },
    { title: '操作', key: 'action', render: (_: any, record: ApprovalInstance) => <a onClick={() => navigate(`/approval/${record.id}`)}>查看</a> },
  ];
  return <ProTable columns={columns} fetchData={(params) => approvalApi.done(params as any)} />;
};

export default DoneApproval;
