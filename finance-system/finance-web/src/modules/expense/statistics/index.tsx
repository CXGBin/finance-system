import React, { useState } from 'react';
import { Card, Select, DatePicker, Button, Table, Statistic, Row, Col } from 'antd';
import { expenseApi } from '@/api/expense';
import dayjs from 'dayjs';

/** 费用统计 */
const ExpenseStatistics: React.FC = () => {
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [params, setParams] = useState({ startDate: dayjs().startOf('month').format('YYYY-MM-DD'), endDate: dayjs().format('YYYY-MM-DD') });

  const loadData = async () => {
    setLoading(true);
    try { const res = await expenseApi.statistics(params); setData(res.data || []); } finally { setLoading(false); }
  };

  const total = data.reduce((s, d) => s + (d.totalAmount || 0), 0);

  const columns = [
    { title: '费用类型', dataIndex: 'expenseTypeName', key: 'expenseTypeName' },
    { title: '报销笔数', dataIndex: 'count', key: 'count', align: 'center' },
    { title: '费用总额', dataIndex: 'totalAmount', key: 'totalAmount', align: 'right' },
  ];

  return (
    <Card title="费用统计">
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col><DatePicker.RangePicker value={[dayjs(params.startDate), dayjs(params.endDate)]} onChange={(dates) => { if (dates) setParams({ startDate: dates[0]!.format('YYYY-MM-DD'), endDate: dates[1]!.format('YYYY-MM-DD') }); }} /></Col>
        <Col><Button type="primary" onClick={loadData} loading={loading}>统计</Button></Col>
      </Row>
      <Statistic title="费用总额" value={total} precision={2} style={{ marginBottom: 16 }} />
      <Table columns={columns} dataSource={data} rowKey="expenseTypeId" loading={loading} pagination={false} />
    </Card>
  );
};

export default ExpenseStatistics;
