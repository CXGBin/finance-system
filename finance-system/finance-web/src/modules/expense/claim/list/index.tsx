import React, { useRef } from 'react';
import { Tag, Space, Button } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { expenseApi } from '@/api/expense';
import { useNavigate } from 'react-router-dom';

/** 报销单列表 */
const ExpenseClaimList: React.FC = () => {
  const navigate = useNavigate();
  const actionRef = useRef<ActionType>();
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '报销单号', dataIndex: 'claimNo', search: true, sorter: true },
    { title: '标题', dataIndex: 'title', search: true },
    { title: '报销金额', dataIndex: 'totalAmount', align: 'right', sorter: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 0: { text: '草稿' }, 1: { text: '审批中' }, 2: { text: '已通过' }, 3: { text: '已驳回' }, 4: { text: '已付款' } }, search: true },
    { title: '提交时间', dataIndex: 'createdTime', sorter: true },
    {
      title: '操作', key: 'action', search: false,
      render: (_, record) => {
        const r = record as any;
        return (
          <Space>
            <a onClick={() => navigate(`/expense/claim/${r.id}`)}>查看</a>
            {r.status === 0 && <a onClick={() => navigate('/expense/claim/add', { state: r })}>编辑</a>}
            {r.status === 0 && <a onClick={() => { expenseApi.claimSubmit(r.id); setTimeout(() => actionRef.current?.reload(), 500); }}>提交</a>}
            {r.status === 2 && <a onClick={() => { expenseApi.paymentConfirm(r.id); setTimeout(() => actionRef.current?.reload(), 500); }}>确认付款</a>}
          </Space>
        );
      },
    },
  ];
  const request = createProTableRequest((params) => expenseApi.claimList(params));
  return (
    <div>
      <ProTable
        actionRef={actionRef}
        columns={columns}
        request={request}
        search={{ labelWidth: 'auto' }}
        rowKey="id"
        toolBarRender={() => [<Button key="add" type="primary" onClick={() => navigate('/expense/claim/add')}>新增报销</Button>]}
      />
    </div>
  );
};

export default ExpenseClaimList;
