import React, { useState } from 'react';
import { Card, Select, Button, Table, Statistic, Row, Col } from 'antd';
import { taxApi } from '@/api/tax';
import dayjs from 'dayjs';

/** 税务报表 */
const TaxReport: React.FC = () => {
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [year, setYear] = useState(dayjs().year());

  const loadData = async () => {
    setLoading(true);
    try { const res = await taxApi.reportList({ year }); setData(res.data || []); } finally { setLoading(false); }
  };

  const columns = [
    { title: '税种', dataIndex: 'taxName', key: 'taxName' },
    { title: '应纳税额', dataIndex: 'taxAmount', key: 'taxAmount', align: 'right' },
    { title: '已缴税额', dataIndex: 'paidAmount', key: 'paidAmount', align: 'right' },
    { title: '未缴税额', dataIndex: 'unpaidAmount', key: 'unpaidAmount', align: 'right' },
  ];

  return (
    <Card title="税务报表">
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col><Select value={year} onChange={setYear} style={{ width: 100 }} options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} /></Col>
        <Col><Button type="primary" onClick={loadData} loading={loading}>查询</Button></Col>
      </Row>
      <Table columns={columns} dataSource={data} rowKey="taxName" loading={loading} pagination={false} />
    </Card>
  );
};

export default TaxReport;
