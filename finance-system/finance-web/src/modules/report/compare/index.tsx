import React, { useState } from 'react';
import { Card, Select, Space, Button, Table, Tag, message } from 'antd';
import { FileExcelOutlined, ReloadOutlined } from '@ant-design/icons';
import { reportCompareApi } from '@/api/report';
import dayjs from 'dayjs';

/** 对比报表 */
const CompareReport: React.FC = () => {
  const [type, setType] = useState('income-statement');
  const [periods, setPeriods] = useState<string[]>([dayjs().format('YYYY-MM'), dayjs().subtract(1, 'month').format('YYYY-MM')]);
  const [displayMode, setDisplayMode] = useState('parallel');
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportCompareApi.compare(type, periods, displayMode); setData(res.data ?? []); } finally { setLoading(false); }
  };

  React.useEffect(() => { loadData(); }, [type, periods, displayMode]);

  const typeName = (t: string) => ({ 'income-statement': '利润表', 'balance-sheet': '资产负债表', 'cash-flow': '现金流量表' }[t] || t);

  const columns = [
    { title: '行次', dataIndex: 'lineNo', width: 60, align: 'center' },
    { title: '项目', dataIndex: 'itemName', ellipsis: true },
    ...periods.map((p, i) => ({
      title: p, dataIndex: `amount_${i}`, key: `amount_${i}`, align: 'right' as const,
      render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-',
    })),
  ];

  return (
    <Card title={`${typeName(type)}对比`} extra={
      <Space>
        <Select value={type} onChange={setType} style={{ width: 140 }}
          options={[{ label: '利润表', value: 'income-statement' }, { label: '资产负债表', value: 'balance-sheet' }, { label: '现金流量表', value: 'cash-flow' }]} />
        <Select mode="multiple" value={periods} onChange={setPeriods} style={{ width: 300 }} maxCount={6}
          options={Array.from({ length: 12 }, (_, i) => ({ label: dayjs().subtract(11 - i, 'month').format('YYYY-MM'), value: dayjs().subtract(11 - i, 'month').format('YYYY-MM') }))} />
        <Select value={displayMode} onChange={setDisplayMode} style={{ width: 100 }}
          options={[{ label: '并列', value: 'parallel' }, { label: '差异', value: 'variance' }]} />
        <Button icon={<ReloadOutlined />} onClick={loadData}>刷新</Button>
        <Button icon={<FileExcelOutlined />} onClick={() => message.info('导出功能开发中')}>导出</Button>
      </Space>
    }>
      <Table columns={columns} dataSource={data} rowKey={(r) => `${r.lineNo}-${r.itemName}`} loading={loading} pagination={false} size="middle" bordered />
    </Card>
  );
};

export default CompareReport;
