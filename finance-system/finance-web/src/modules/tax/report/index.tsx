import React, { useState } from 'react';
import { Card, Select, Space, Button, Statistic, Row, Col, Table, Spin, Alert, Empty } from 'antd';
import { ReloadOutlined, FileExcelOutlined } from '@ant-design/icons';
import { taxApi } from '@/api/tax';
import dayjs from 'dayjs';

/** 纳税统计 */
const TaxReport: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try { const res = await taxApi.declarationList({ pageIndex: 1, pageSize: 999 }); setData(res.data?.list ?? []); } catch (err: unknown) { setError(err instanceof Error ? err.message : '加载纳税统计数据失败'); } finally { setLoading(false); }
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
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadData}>重试</Button>} />
        ) : (
          <Table columns={columns} dataSource={data} rowKey="id" pagination={false} locale={{ emptyText: <Empty description="暂无纳税数据" /> }} />
        )}
      </Spin>
    </Card>
  );
};

export default TaxReport;
