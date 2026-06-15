import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Tag, message, Popconfirm, Space, Select, InputNumber } from 'antd';
import { periodApi } from '@/api/account';
import dayjs from 'dayjs';
import type { AccountingPeriod } from '@/types/account.d';

/** 会计期间管理 */
const PeriodList: React.FC = () => {
  const [year, setYear] = useState(dayjs().year());
  const [data, setData] = useState<AccountingPeriod[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadData(); }, [year]);

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await periodApi.list(year);
      setData(res.data || []);
    } finally { setLoading(false); }
  };

  const handleClose = async (record: AccountingPeriod) => {
    await periodApi.close(`${record.periodYear}-${String(record.periodMonth).padStart(2, '0')}`);
    message.success('结账成功');
    loadData();
  };

  const handleReopen = async (record: AccountingPeriod) => {
    await periodApi.reopen(`${record.periodYear}-${String(record.periodMonth).padStart(2, '0')}`);
    message.success('反结账成功');
    loadData();
  };

  const handleCarryForward = async (record: AccountingPeriod) => {
    await periodApi.carryForward(`${record.periodYear}-${String(record.periodMonth).padStart(2, '0')}`);
    message.success('期末结转成功');
    loadData();
  };

  const columns = [
    { title: '年度', dataIndex: 'periodYear', key: 'periodYear' },
    { title: '月份', dataIndex: 'periodMonth', key: 'periodMonth' },
    { title: '开始日期', dataIndex: 'beginDate', key: 'beginDate' },
    { title: '结束日期', dataIndex: 'endDate', key: 'endDate' },
    {
      title: '状态', dataIndex: 'isClosed', key: 'isClosed',
      render: (val: number) => val === 1 ? <Tag color="success">已结账</Tag> : <Tag color="processing">未结账</Tag>,
    },
    { title: '结账时间', dataIndex: 'closedTime', key: 'closedTime' },
    {
      title: '操作', key: 'action',
      render: (_: unknown, record: AccountingPeriod) => (
        <Space>
          {record.isClosed === 0 && (
            <>
              <Popconfirm title="确认结账?" onConfirm={() => handleClose(record as AccountingPeriod)}><a>结账</a></Popconfirm>
              <Popconfirm title="确认期末结转?" onConfirm={() => handleCarryForward(record as AccountingPeriod)}><a>结转</a></Popconfirm>
            </>
          )}
          {record.isClosed === 1 && (
            <Popconfirm title="确认反结账?" onConfirm={() => handleReopen(record as AccountingPeriod)}><a>反结账</a></Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <Card title="会计期间">
      <Space style={{ marginBottom: 16 }}>
        <span>年度：</span>
        <InputNumber value={year} min={2020} max={2099} onChange={(v) => v && setYear(v)} />
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading} pagination={false} />
    </Card>
  );
};

export default PeriodList;
