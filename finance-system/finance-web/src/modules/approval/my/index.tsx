import React from 'react';
import { Tag, Button, Space } from 'antd';
import ProTable from '@/components/ProTable';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';
import type { ApprovalInstance } from '@/types/approval.d';

/** 我的申请页面 */
const MyApproval: React.FC = () => {
  const navigate = useNavigate();
  const columns = [
    { title: '审批单号', dataIndex: 'approvalNo', key: 'approvalNo', search: true },
    { title: '标题', dataIndex: 'title', key: 'title' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => {
        const map: Record<number, { color: string; text: string }> = { 0: { color: 'default', text: '审批中' }, 1: { color: 'success', text: '已通过' }, 2: { color: 'error', text: '已驳回' } };
        const info = map[val] || { color: 'default', text: '未知' };
        return <Tag color={info.color}>{info.text}</Tag>;
      },
    },
    { title: '提交时间', dataIndex: 'createdTime', key: 'createdTime' },
    {
      title: '操作', key: 'action', render: (_: unknown, record: ApprovalInstance) => (
        <Space>
          <a onClick={() => navigate(`/approval/${record.id}`)}>详情</a>
          {record.status === 0 && <a onClick={() => approvalApi.withdraw(record.id).then(() => window.location.reload())}>撤回</a>}
        </Space>
      ),
    },
  ];
  return <ProTable columns={columns} fetchData={(params) => approvalApi.my(params as any)} />;
};

export default MyApproval;
