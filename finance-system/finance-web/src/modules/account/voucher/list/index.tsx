import React, { useState, useCallback } from 'react';
import { Card, Input, Button, Space, message, Tag, Popconfirm } from 'antd';
import { voucherApi, voucherBatchApi } from '@/api/account';
import type { PageParams } from '@/types/api.d';
import { useNavigate } from 'react-router-dom';
import type { Voucher } from '@/types/account.d';
import ProTable from '@/components/ProTable';
import type { ProTableRef } from '@/components/ProTable';

/** 凭证列表页面 */
const VoucherList: React.FC = () => {
  const [searchParams, setSearchParams] = useState<Record<string, string>>({});
  const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([]);
  const [tableKey, setTableKey] = useState(0);
  const tableRef = React.useRef<ProTableRef>(null);
  const navigate = useNavigate();

  const handleAudit = async (id: number) => {
    await voucherApi.audit(id);
    message.success('审核成功');
    tableRef.current?.refresh();
  };

  const handleVoid = async (id: number) => {
    await voucherApi.void(id);
    message.success('已作废');
    tableRef.current?.refresh();
  };

  /** 批量审核凭证 */
  const handleBatchAudit = async () => {
    try {
      await voucherBatchApi.batchAudit(selectedRowKeys);
      message.success('批量审核成功');
      setSelectedRowKeys([]);
      tableRef.current?.refresh();
    } catch { message.error('批量审核失败'); }
  };

  /** 复制凭证 */
  const handleCopy = async (id: number) => {
    try {
      const res = await voucherBatchApi.copy(id);
      message.success('复制成功');
      navigate(`/account/voucher/add`, { state: { id: res.data } });
    } catch { message.error('复制失败'); }
  };

  /** 红字冲销凭证 */
  const handleReverse = async (id: number) => {
    try {
      await voucherBatchApi.reverse(id);
      message.success('红字冲销凭证已生成');
      tableRef.current?.refresh();
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
          {record.status === 2 && <a onClick={() => handleCopy(record.id)}>复制</a>}
        </Space>
      ),
    },
  ];

  const handleSearch = useCallback(() => {
    setTableKey((k) => k + 1);
  }, []);

  return (
    <Card title="凭证管理" extra={(
      <Space>
        <Button type="primary" onClick={() => navigate('/account/voucher/add')}>新增凭证</Button>
        {selectedRowKeys.length > 0 && <Button onClick={handleBatchAudit}>批量审核({selectedRowKeys.length})</Button>}
      </Space>
    )}>
      <Space style={{ marginBottom: 16 }}>
        <Input placeholder="凭证字号" value={searchParams.voucherNo} onChange={(e) => setSearchParams({ ...searchParams, voucherNo: e.target.value })} allowClear style={{ width: 150 }} />
        <Button type="primary" onClick={handleSearch}>查询</Button>
      </Space>
      <ProTable<Voucher>
        key={tableKey}
        ref={tableRef}
        columns={columns}
        fetchData={async (params) => {
          const res = await voucherApi.page({ page: params.pageIndex, pageSize: params.pageSize, ...searchParams } as PageParams);
          return { data: { list: res.list || [], total: res.total || 0 } };
        }}
        searchInitialValues={searchParams}
        tableProps={{
          rowSelection: {
            selectedRowKeys,
            onChange: (keys) => setSelectedRowKeys(keys as number[]),
          },
        }}
      />
    </Card>
  );
};

export default VoucherList;
