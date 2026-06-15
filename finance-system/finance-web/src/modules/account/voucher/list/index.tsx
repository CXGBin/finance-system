import React, { useState, useEffect } from 'react';
import { Card, Table, Input, Button, Space, message, Tag, Popconfirm } from 'antd';
import { DatePicker } from 'antd';
import { voucherApi } from '@/api/account';
import { useNavigate } from 'react-router-dom';
import type { Voucher } from '@/types/account.d';

/** 凭证列表页面 */
const VoucherList: React.FC = () => {
  const [data, setData] = useState<Voucher[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [searchParams, setSearchParams] = useState<any>({});
  const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([]);
  const navigate = useNavigate();

  useEffect(() => { loadData(); }, [page, pageSize]);

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await voucherApi.list({ page, pageSize, ...searchParams });
      const pageData = res.data as { list?: typeof data; total?: number };
      setData(pageData.list || (Array.isArray(res.data) ? res.data : []));
      setTotal(pageData.total || 0);
    } finally { setLoading(false); }
  };

  const handleAudit = async (id: number) => {
    await voucherApi.audit(id);
    message.success('审核成功');
    loadData();
  };

  const handleVoid = async (id: number) => {
    await voucherApi.void(id);
    message.success('已作废');
    loadData();
  };

  const handleBatchAudit = async () => {
    try {
      await fetch('/api/voucher/batch-audit', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(selectedRowKeys) });
      message.success('批量审核成功');
      setSelectedRowKeys([]);
      loadData();
    } catch { message.error('批量审核失败'); }
  };

  const handleReverse = async (id: number) => {
    try {
      await fetch('/api/voucher/' + id + '/reverse', { method: 'POST' });
      message.success('红字冲销凭证已生成');
      loadData();
    } catch { message.error('冲销失败'); }
  };

  const columns = [
    { title: '凭证字号', dataIndex: 'voucherNo', key: 'voucherNo' },
    { title: '凭证日期', dataIndex: 'voucherDate', key: 'voucherDate' },
    { title: '摘要', dataIndex: 'summary', key: 'summary', ellipsis: true },
    { title: '借方合计', dataIndex: 'totalDebit', key: 'totalDebit', align: 'right' },
    { title: '贷方合计', dataIndex: 'totalCredit', key: 'totalCredit', align: 'right' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => {
        const map: Record<number, { color: string; text: string }> = {
          0: { color: 'default', text: '草稿' }, 1: { color: 'processing', text: '待审核' },
          2: { color: 'success', text: '已审核' }, 3: { color: 'error', text: '已作废' },
        };
        const info = map[val] || { color: 'default', text: '未知' };
        return <Tag color={info.color}>{info.text}</Tag>;
      },
    },
    {
      title: '操作', key: 'action', render: (_: unknown, record: Voucher) => (
        <Space>
          <a onClick={() => navigate(`/account/voucher/${record.id}`)}>查看</a>
          {record.status === 0 && <a onClick={() => navigate(`/account/voucher/add`, { state: record })}>编辑</a>}
          {record.status === 1 && <Popconfirm title="确认审核?" onConfirm={() => handleAudit(record.id)}><a>审核</a></Popconfirm>}
          {(record.status === 0 || record.status === 1) && <Popconfirm title="确认作废?" onConfirm={() => handleVoid(record.id)}><a style={{ color: '#ff4d4f' }}>作废</a></Popconfirm>}
          {record.status === 2 && <Popconfirm title="确认红字冲销该凭证?" onConfirm={() => handleReverse(record.id)}><a style={{ color: '#faad14' }}>冲销</a></Popconfirm>}
        </Space>
      ),
    },
  ];

  return (
    <Card title="凭证管理" extra={(
      <Space>
        <Button type="primary" onClick={() => navigate('/account/voucher/add')}>新增凭证</Button>
        {selectedRowKeys.length > 0 && <Button onClick={handleBatchAudit}>批量审核({selectedRowKeys.length})</Button>}
      </Space>
    )}>
      <Space style={{ marginBottom: 16 }}>
        <Input placeholder="凭证字号" value={searchParams.voucherNo} onChange={(e) => setSearchParams({ ...searchParams, voucherNo: e.target.value })} allowClear style={{ width: 150 }} />
        <Button type="primary" onClick={() => { setPage(1); loadData(); }}>查询</Button>
      </Space>
      <Table columns={columns} dataSource={data} rowKey="id" loading={loading}
        rowSelection={{ selectedRowKeys, onChange: (keys) => setSelectedRowKeys(keys as number[]) }}
        pagination={{ current: page, pageSize, total, showSizeChanger: true, onChange: (p, ps) => { setPage(p); setPageSize(ps); }, showTotal: (t) => `共 ${t} 条` }}
      />
    </Card>
  );
};

export default VoucherList;
