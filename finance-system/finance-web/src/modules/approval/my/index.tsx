import React, { useState, useRef } from 'react';
import { Tag, Tabs } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { approvalApi } from '@/api/approval';
import { useNavigate } from 'react-router-dom';

/** 我的申请页面 */
const MyApproval: React.FC = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<string>('initiated');
  const actionRef = useRef<ActionType>();

  const commonColumns: ProColumns<Record<string, unknown>>[] = [
    { title: '业务ID', dataIndex: 'businessId', search: true, sorter: true },
    { title: '标题', dataIndex: 'title', search: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 0: { text: '审批中' }, 1: { text: '已通过' }, 2: { text: '已驳回' } }, search: true },
    { title: '提交时间', dataIndex: 'createdTime', sorter: true },
  ];

  const requestInitiated = createProTableRequest((params) => approvalApi.list({ ...params, type: 'mine-initiated' }));
  const requestApproved = createProTableRequest((params) => approvalApi.list({ ...params, type: 'mine-approved' }));

  const tabItems = [
    {
      key: 'initiated',
      label: '我发起的',
      children: (
        <ProTable
          actionRef={actionRef}
          columns={[
            ...commonColumns,
            {
              title: '操作', key: 'action', search: false,
              render: (_, record) => {
                const r = record as any;
                return (
                  <>
                    <a onClick={() => navigate(`/approval/${r.id}`)}>详情</a>
                    {r.status === 0 && <a onClick={() => { approvalApi.withdraw(r.id); setTimeout(() => actionRef.current?.reload(), 500); }}>撤回</a>}
                  </>
                );
              },
            },
          ]}
          request={requestInitiated}
          search={{ labelWidth: 'auto', defaultCollapsed: true }}
          rowKey="id"
        />
      ),
    },
    {
      key: 'approved',
      label: '我审批的',
      children: (
        <ProTable
          actionRef={actionRef}
          columns={[
            ...commonColumns,
            {
              title: '操作', key: 'action', search: false,
              render: (_, record) => <a onClick={() => navigate(`/approval/${(record as any).id}`)}>详情</a>,
            },
          ]}
          request={requestApproved}
          search={{ labelWidth: 'auto', defaultCollapsed: true }}
          rowKey="id"
        />
      ),
    },
  ];

  return <Tabs activeKey={activeTab} onChange={setActiveTab} items={tabItems} />;
};

export default MyApproval;
