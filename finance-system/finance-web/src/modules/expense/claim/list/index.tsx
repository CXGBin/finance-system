import React from 'react';
import { Tag, Space, Button } from 'antd';
import ProTable from '@/components/ProTable';
import { expenseApi } from '@/api/expense';
import { useNavigate } from 'react-router-dom';
import type { ExpenseClaim } from '@/types/expense.d';

/** 报销单列表 */
const ExpenseClaimList: React.FC = () => {
  const navigate = useNavigate();
  const columns = [
    { title: '报销单号', dataIndex: 'claimNo', key: 'claimNo', search: true },
    { title: '标题', dataIndex: 'title', key: 'title' },
    { title: '报销金额', dataIndex: 'totalAmount', key: 'totalAmount', align: 'right' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => {
        const map: Record<number, { color: string; text: string }> = { 0: { color: 'default', text: '草稿' }, 1: { color: 'processing', text: '审批中' }, 2: { color: 'success', text: '已通过' }, 3: { color: 'error', text: '已驳回' }, 4: { color: 'blue', text: '已付款' } };
        const info = map[val] || { color: 'default', text: '未知' };
        return <Tag color={info.color}>{info.text}</Tag>;
      },
    },
    { title: '提交时间', dataIndex: 'createdTime', key: 'createdTime' },
    {
      title: '操作', key: 'action', render: (_: any, record: ExpenseClaim) => (
        <Space>
          <a onClick={() => navigate(`/expense/claim/${record.id}`)}>查看</a>
          {record.status === 0 && <a onClick={() => navigate('/expense/claim/add', { state: record })}>编辑</a>}
          {record.status === 0 && <a onClick={() => expenseApi.claimSubmit(record.id).then(() => window.location.reload())}>提交</a>}
          {record.status === 2 && <a onClick={() => expenseApi.paymentConfirm(record.id).then(() => window.location.reload())}>确认付款</a>}
        </Space>
      ),
    },
  ];
  return (
    <div>
      <ProTable columns={columns} fetchData={(params) => expenseApi.claimList(params as any)} toolbarActions={<Button type="primary" onClick={() => navigate('/expense/claim/add')}>新增报销</Button>} />
    </div>
  );
};

export default ExpenseClaimList;
