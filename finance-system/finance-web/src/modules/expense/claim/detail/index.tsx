import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Table, Tag, Button } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { expenseApi } from '@/api/expense';
import type { ExpenseClaim } from '@/types/expense.d';

/** 报销详情 */
const ExpenseClaimDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<ExpenseClaim | null>(null);

  useEffect(() => { if (id) expenseApi.claimDetail(Number(id)).then(res => setData(res.data || null)); }, [id]);

  const columns = [
    { title: '费用类型', dataIndex: 'expenseTypeName', key: 'expenseTypeName' },
    { title: '说明', dataIndex: 'description', key: 'description' },
    { title: '金额', dataIndex: 'amount', key: 'amount', align: 'right' },
    { title: '费用日期', dataIndex: 'expenseDate', key: 'expenseDate' },
    { title: '发票号', dataIndex: 'invoiceNo', key: 'invoiceNo' },
  ];

  return (
    <Card title="报销详情" extra={<Button onClick={() => navigate(-1)}>返回</Button>}>
      {data && (<>
        <Descriptions bordered size="small" column={3}>
          <Descriptions.Item label="报销单号">{data.claimNo}</Descriptions.Item>
          <Descriptions.Item label="标题">{data.title}</Descriptions.Item>
          <Descriptions.Item label="状态"><Tag>{['草稿', '审批中', '已通过', '已驳回', '已付款'][data.status]}</Tag></Descriptions.Item>
          <Descriptions.Item label="总金额">{data.totalAmount?.toFixed(2)}</Descriptions.Item>
          <Descriptions.Item label="付款日期">{data.paymentDate || '-'}</Descriptions.Item>
        </Descriptions>
        <Table columns={columns} dataSource={(data as any).items || []} rowKey="id" pagination={false} style={{ marginTop: 16 }} />
      </>)}
    </Card>
  );
};

export default ExpenseClaimDetail;
