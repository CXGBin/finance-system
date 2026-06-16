import React, { useState } from 'react';
import { Tag, Button, Space, Tabs } from 'antd';
import ProTable from '@/components/ProTable';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';
import type { ApprovalInstance } from '@/types/approval.d';
import type { PageParams } from '@/types/api.d';

/** 我的申请页面 */
const MyApproval: React.FC = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<string>('initiated');

  const commonColumns = [
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
  ];

  const initiatedColumns = [
    ...commonColumns,
    {
      title: '操作', key: 'action', render: (_: unknown, record: ApprovalInstance) => (
        <Space>
          <a onClick={() => navigate(`/approval/${record.id}`)}>详情</a>
          {record.status === 0 && <a onClick={() => approvalApi.withdraw(record.id).then(() => window.location.reload())}>撤回</a>}
        </Space>
      ),
    },
  ];

  const approvedColumns = [
    ...commonColumns,
    {
      title: '操作', key: 'action', render: (_: unknown, record: ApprovalInstance) => (
        <a onClick={() => navigate(`/approval/${record.id}`)}>详情</a>
      ),
    },
  ];

  /** 我发起的 */
  const fetchInitiated = (params: PageParams) => {
    return approvalApi.list({ ...params, type: 'mine-initiated' });
  };

  /** 我审批的 */
  const fetchApproved = (params: PageParams) => {
    return approvalApi.list({ ...params, type: 'mine-approved' });
  };

  const tabItems = [
    {
      key: 'initiated',
      label: '我发起的',
      children: <ProTable columns={initiatedColumns} fetchData={fetchInitiated as any} />,
    },
    {
      key: 'approved',
      label: '我审批的',
      children: <ProTable columns={approvedColumns} fetchData={fetchApproved as any} />,
    },
  ];

  return (
    <div>
      <Tabs activeKey={activeTab} onChange={setActiveTab} items={tabItems} />
    </div>
  );
};

export default MyApproval;
