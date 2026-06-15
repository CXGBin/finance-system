import React, { useState, useEffect } from 'react';
import { Card, Table, Select, Button, Space, message } from 'antd';
import { reportApi } from '@/api/report';

/** 自定义报表页面 */
const CustomReport: React.FC = () => {
  const [reportId, setReportId] = useState<string>('');
  const [reportIds, setReportIds] = useState<{ label: string; value: string }[]>([]);
  const [data, setData] = useState<Record<string, unknown>[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadIds(); }, []);

  const loadIds = async () => {
    try {
      const res = await reportApi.templateList({ pageIndex: 1, pageSize: 100 });
      const pageData = res.data as { list?: Array<{ templateName?: string; name?: string; id: number }> } | Array<{ templateName?: string; name?: string; id: number }>;
      const list = Array.isArray(pageData) ? pageData : (pageData as { list?: Array<{ templateName?: string; name?: string; id: number }> }).list || [];
      setReportIds(list.map((t) => ({ label: t.templateName || t.name || '', value: String(t.id) })));
    } catch {
      setReportIds([]);
    }
  };

  const loadData = async () => {
    if (!reportId) return;
    setLoading(true);
    try { const res = await reportApi.custom({ reportId }); setData((res.data as Record<string, unknown>[]) || []); } finally { setLoading(false); }
  };

  const columns = [
    { title: '项目', dataIndex: 'itemName', key: 'itemName' },
    { title: '数值', dataIndex: 'value', key: 'value', align: 'right' },
  ];

  return (
    <Card title="自定义报表">
      <Space style={{ marginBottom: 16 }}>
        <Select placeholder="选择报表" value={reportId} onChange={setReportId} style={{ width: 200 }} options={reportIds} />
        <Button type="primary" onClick={loadData} loading={loading}>生成</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="itemName" loading={loading} pagination={false} />
    </Card>
  );
};

export default CustomReport;
