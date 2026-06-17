import React, { useState } from 'react';
import { Card, Select, DatePicker, Button, Row, Col, Statistic, Table, Space, Spin, Alert, Empty } from 'antd';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons';
import { expenseApi } from '@/api/expense';
import dayjs from 'dayjs';

/** 费用统计 */
const ExpenseStatistics: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [data, setData] = useState<any[]>([]);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await expenseApi.statistics({ year });
      setData(res.data?.list ?? res.data ?? []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载费用统计数据失败');
    } finally { setLoading(false); }
  };

  React.useEffect(() => { loadData(); }, [year]);

  const totalExpense = data.reduce((s: number, r: any) => s + (r.totalAmount ?? 0), 0);
  const totalClaim = data.reduce((s: number, r: any) => s + (r.claimCount ?? 0), 0);

  const columns = [
    { title: '费用类型', dataIndex: 'typeName', ellipsis: true },
    { title: '报销单数', dataIndex: 'claimCount', align: 'right' },
    { title: '总金额', dataIndex: 'totalAmount', align: 'right', render: (v: number) => <span className="amount-right">¥{(v ?? 0).toFixed(2)}</span> },
    { title: '已审批', dataIndex: 'approvedAmount', align: 'right', render: (v: number) => <span className="amount-right">¥{(v ?? 0).toFixed(2)}</span> },
    { title: '待审批', dataIndex: 'pendingAmount', align: 'right', render: (v: number) => <span className="amount-right">¥{(v ?? 0).toFixed(2)}</span> },
  ];

  return (
    <Card title="费用统计" extra={
      <Space>
        <Select value={year} onChange={setYear} style={{ width: 120 }}
          options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <Button icon={<ReloadOutlined />} onClick={loadData}>刷新</Button>
      </Space>
    }>
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={8}><Statistic title="费用总额" value={totalExpense} precision={2} prefix="¥" /></Col>
        <Col span={8}><Statistic title="报销单数" value={totalClaim} suffix="单" /></Col>
        <Col span={8}><Statistic title="人均费用" value={totalClaim > 0 ? totalExpense / totalClaim : 0} precision={2} prefix="¥" /></Col>
      </Row>
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadData}>重试</Button>} />
        ) : (
          <Table columns={columns} dataSource={data} rowKey={(r) => r.typeName} pagination={false} locale={{ emptyText: <Empty description="暂无统计数据" /> }} />
        )}
      </Spin>
    </Card>
  );
};

export default ExpenseStatistics;
