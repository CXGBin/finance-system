import React, { useState } from 'react';
import { Card, Select, Space, Button, Table, Switch, message } from 'antd';
import { FileExcelOutlined, ReloadOutlined } from '@ant-design/icons';
import { reportApi, reportExportApi } from '@/api/report';
import dayjs from 'dayjs';

/** 利润表 */
const IncomeStatement: React.FC = () => {
  const [period, setPeriod] = useState(dayjs().format('YYYY-MM'));
  const [showZero, setShowZero] = useState(false);
  const [dataType, setDataType] = useState('month');
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const loadData = async () => {
    setLoading(true);
    try { const res = await reportApi.incomeStatement(period, showZero, dataType); setData(res.data?.items ?? res.data ?? []); } finally { setLoading(false); }
  };

  React.useEffect(() => { loadData(); }, [period, showZero, dataType]);

  const handleExport = async () => {
    try { await reportExportApi.exportExcel('income-statement', period); message.success('导出成功'); } catch { /* error */ }
  };

  const columns = [
    { title: '行次', dataIndex: 'lineNo', width: 60, align: 'center' },
    { title: '项目', dataIndex: 'itemName', ellipsis: true },
    { title: '本月数', dataIndex: 'currentAmount', align: 'right', render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-' },
    { title: '本年累计', dataIndex: 'yearAmount', align: 'right', render: (v: number) => v ? <span className="amount-right">{v.toFixed(2)}</span> : '-' },
  ];

  return (
    <Card title="利润表" extra={
      <Space>
        <Space size={4}><span>期间：</span>
          <Select value={period} onChange={setPeriod} style={{ width: 140 }}
            options={Array.from({ length: 12 }, (_, i) => ({ label: dayjs().subtract(11 - i, 'month').format('YYYY-MM'), value: dayjs().subtract(11 - i, 'month').format('YYYY-MM') }))} />
        </Space>
        <Select value={dataType} onChange={setDataType} style={{ width: 100 }} options={[{ label: '月度', value: 'month' }, { label: '年度', value: 'year' }]} />
        <Space size={4}><span>显示零值：</span><Switch size="small" checked={showZero} onChange={setShowZero} /></Space>
        <Button icon={<ReloadOutlined />} onClick={loadData}>刷新</Button>
        <Button icon={<FileExcelOutlined />} onClick={handleExport}>导出</Button>
      </Space>
    }>
      <Table columns={columns} dataSource={data} rowKey={(r) => `${r.lineNo}-${r.itemName}`} loading={loading} pagination={false} size="middle" bordered />
    </Card>
  );
};

export default IncomeStatement;
