import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Table, Tag, Button, Spin, Alert, Empty } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { expenseApi } from '@/api/expense';
import type { ExpenseClaim } from '@/types/expense.d';

/** 报销详情 */
const ExpenseClaimDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<ExpenseClaim | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      setLoading(true);
      setError(null);
      expenseApi.claimDetail(Number(id))
        .then(res => setData(res.data || null))
        .catch((err: unknown) => setError(err instanceof Error ? err.message : '加载报销详情失败'))
        .finally(() => setLoading(false));
    }
  }, [id]);

  const columns = [
    { title: '费用类型', dataIndex: 'expenseTypeName', key: 'expenseTypeName' },
    { title: '说明', dataIndex: 'description', key: 'description' },
    { title: '金额', dataIndex: 'amount', key: 'amount', align: 'right' },
    { title: '费用日期', dataIndex: 'expenseDate', key: 'expenseDate' },
    { title: '发票号', dataIndex: 'invoiceNo', key: 'invoiceNo' },
  ];

  return (
    <Card title="报销详情" extra={<Button onClick={() => navigate(-1)}>返回</Button>}>
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={() => id && expenseApi.claimDetail(Number(id)).then(res => setData(res.data || null))}>重试</Button>} />
        ) : data ? (<>
          <Descriptions bordered size="small" column={3}>
            <Descriptions.Item label="报销单号">{data.claimNo}</Descriptions.Item>
            <Descriptions.Item label="标题">{data.title}</Descriptions.Item>
            <Descriptions.Item label="状态"><Tag>{['草稿', '审批中', '已通过', '已驳回', '已付款'][data.status]}</Tag></Descriptions.Item>
            <Descriptions.Item label="总金额">{data.totalAmount?.toFixed(2)}</Descriptions.Item>
            <Descriptions.Item label="付款日期">{data.paymentDate || '-'}</Descriptions.Item>
          </Descriptions>
          <Table columns={columns} dataSource={(data as any).items || []} rowKey="id" pagination={false} style={{ marginTop: 16 }} />
        </>) : !loading ? (
          <Empty description="暂无报销数据" />
        ) : null}
      </Spin>
    </Card>
  );
};

export default ExpenseClaimDetail;
