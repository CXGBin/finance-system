import React, { useState, useEffect } from 'react';
import { Card, Table, Select, Button, Space, message, Tag } from 'antd';
import { reportCompareApi, type CompareRow } from '@/api/report';
import dayjs from 'dayjs';

/** 对比报表页面 */
const CompareReport: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [selectedPeriods, setSelectedPeriods] = useState<string[]>([]);
  const [reportType, setReportType] = useState('income');
  const [data, setData] = useState<CompareRow[]>([]);
  const [loading, setLoading] = useState(false);

  // 生成年内所有月份选项
  const periodOptions = Array.from({ length: 12 }, (_, i) => ({
    label: `${year}年${i + 1}月`,
    value: `${year}-${String(i + 1).padStart(2, '0')}`,
  }));

  const handleQuery = async () => {
    if (selectedPeriods.length < 2) {
      message.warning('请至少选择2个会计期间');
      return;
    }
    if (selectedPeriods.length > 4) {
      message.warning('最多选择4个会计期间');
      return;
    }
    setLoading(true);
    try {
      const res = await reportCompareApi.compare({ reportType, periods: selectedPeriods });
      setData(res.data || []);
    } catch {
      message.error('加载对比数据失败');
    } finally {
      setLoading(false);
    }
  };

  // 动态列
  const columns = [
    { title: '项目', dataIndex: 'itemName', key: 'itemName', fixed: 'left' as const, width: 200 },
    ...selectedPeriods.map((period, idx) => ({
      title: period,
      dataIndex: `period_${idx}`,
      key: `period_${idx}`,
      align: 'right' as const,
      width: 140,
      render: (_: unknown, record: CompareRow) => {
        const p = record.periods?.find((item) => item.period === period);
        return p ? Number(p.amount).toFixed(2) : '-';
      },
    })),
  ];

  return (
    <Card title="对比报表">
      <Space style={{ marginBottom: 16 }} wrap>
        <Select
          value={reportType}
          onChange={setReportType}
          style={{ width: 150 }}
          options={[
            { label: '利润表', value: 'income' },
            { label: '资产负债表', value: 'balance-sheet' },
            { label: '现金流量表', value: 'cash-flow' },
          ]}
        />
        <Select
          value={year}
          onChange={(v) => { setYear(v); setSelectedPeriods([]); }}
          style={{ width: 100 }}
          options={Array.from({ length: 5 }, (_, i) => ({ label: String(dayjs().year() - 2 + i), value: dayjs().year() - 2 + i }))}
        />
        <span>期间：</span>
        <Select
          mode="multiple"
          value={selectedPeriods}
          onChange={setSelectedPeriods}
          style={{ minWidth: 300 }}
          maxTagCount={4}
          placeholder="选择2-4个期间"
          options={periodOptions}
        />
        <Button type="primary" onClick={handleQuery} loading={loading}>查询</Button>
      </Space>
      <Table
        columns={columns}
        dataSource={data}
        rowKey="itemName"
        loading={loading}
        scroll={{ x: 200 + selectedPeriods.length * 140 }}
        pagination={false}
      />
    </Card>
  );
};

export default CompareReport;
