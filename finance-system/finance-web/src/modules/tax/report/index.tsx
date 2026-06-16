import React, { useState } from 'react';
import { Card, Select, Space, Button, Statistic, Row, Col, Table } from 'antd';
import { ReloadOutlined, FileExcelOutlined } from '@ant-design/icons';
import { taxApi } from '@/api/tax';
import dayjs from 'dayjs';

/** 纳税统计 */
const TaxReport: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [data, setData] = useState<any[]>([]);

  const loadData = async () => {
    try { const res = await taxApi.declarationList({ pageIndex: 1, pageSize: 999 }); setData(res.data?.list ?? []); } catch { /* error */ }
  };

  React.useEffect(() => { loadData(); }, []);

  const columns = [
    { title: '税种', dataIndex: 'taxName' },
    { title: '申报期间', dataIndex: 'declarePeriod' },
    { title: '应纳税额', dataIndex: 'taxAmount', align: 'right', render: (v: number) => <span className="amount-right">{(v ?? 0).toFixed(2)}</span> },
    { title: '实缴税额', dataIndex: 'actualPaidAmount', align: 'right', render: (v: number) => <span className="amount-right">{(v ?? 0).toFixed(2)}</span> },
  ];

  return (
    <Card title="纳税统计" extra={
      <Space>
        <Select value={year} onChange={setYear} style={{ width: 120 }}
          options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))} />
        <Button icon={<ReloadOutlined />} onClick={loadData}>刷新</Button>
      </Space>
    }>
      <Table columns={columns} dataSource={data} rowKey="id" pagination={false} />
    </Card>
  );
};

export default TaxReport;
