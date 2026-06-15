import React, { useState, useEffect } from 'react';
import { Card, Table, Select, Button, Space, message } from 'antd';
import { reportApi } from '@/api/report';

/** 自定义报表页面 */
const CustomReport: React.FC = () => {
  const [reportId, setReportId] = useState<string>('');
  const [reportIds, setReportIds] = useState<any[]>([]);
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadIds(); }, []);

  const loadIds = async () => {
    // TODO: 从报表模板API获取列表
    setReportIds([{ label: '自定义报表1', value: '1' }]);
  };

  const loadData = async () => {
    if (!reportId) return;
    setLoading(true);
    try { const res = await reportApi.custom({ reportId }); setData((res.data as any) || []); } finally { setLoading(false); }
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
