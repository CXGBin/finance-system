import React, { useRef } from 'react';
import { Tag, Space, Button, Popconfirm, message } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import type { Voucher } from '@/types/account.d';
import { voucherApi } from '@/api/account';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 凭证列表页面 */
const VoucherList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const navigate = useNavigate();

  const columns: ProColumns<Voucher>[] = [
    {
      title: '凭证号',
      dataIndex: 'voucherNo',
      ellipsis: true,
    },
    {
      title: '凭证类型',
      dataIndex: 'voucherType',
      valueType: 'select',
      valueEnum: {
        0: { text: '记账凭证', status: 'Default' },
        1: { text: '收款凭证', status: 'Success' },
        2: { text: '付款凭证', status: 'Warning' },
        3: { text: '转账凭证', status: 'Processing' },
      },
    },
    {
      title: '摘要',
      dataIndex: 'abstractText',
      ellipsis: true,
      search: false,
    },
    {
      title: '凭证日期',
      dataIndex: 'voucherDate',
      valueType: 'date',
      sorter: true,
      search: false,
      width: 120,
    },
    {
      title: '借方合计',
      dataIndex: 'debitAmount',
      align: 'right',
      search: false,
      render: (_, record) => <span className="amount-right">{(record.debitAmount ?? 0).toFixed(2)}</span>,
    },
    {
      title: '贷方合计',
      dataIndex: 'creditAmount',
      align: 'right',
      search: false,
      render: (_, record) => <span className="amount-right">{(record.creditAmount ?? 0).toFixed(2)}</span>,
    },
    {
      title: '状态',
      dataIndex: 'status',
      valueType: 'select',
      valueEnum: {
        0: { text: '草稿', status: 'Default' },
        1: { text: '已审核', status: 'Success' },
        2: { text: '已作废', status: 'Error' },
      },
    },
    {
      title: '创建时间',
      dataIndex: 'createdTime',
      valueType: 'dateTime',
      sorter: true,
      search: false,
      width: 180,
    },
    {
      title: '操作',
      valueType: 'option',
      width: 280,
      render: (_, record) => (
        <Space>
          <a onClick={() => navigate(`/account/voucher/${record.id}`)}>查看</a>
          {record.status === 0 && <a onClick={() => navigate('/account/voucher/add', { state: record })}>编辑</a>}
          {record.status === 0 && <Popconfirm title="确认审核?" onConfirm={() => handleAudit(record)}><a>审核</a></Popconfirm>}
          {record.status === 1 && <Popconfirm title="确认反审核?" onConfirm={() => handleUnaudit(record)}><a>反审</a></Popconfirm>}
          {record.status === 0 && <Popconfirm title="确认作废?" onConfirm={() => handleVoid(record)}><a style={{ color: '#ff4d4f' }}>作废</a></Popconfirm>}
          <Popconfirm title="确认删除?" onConfirm={() => handleDelete(record)}><a style={{ color: '#ff4d4f' }}>删除</a></Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAudit = async (record: Voucher) => {
    await voucherApi.audit(record.id);
    message.success('审核成功');
    actionRef.current?.reload();
  };

  const handleUnaudit = async (record: Voucher) => {
    await voucherApi.unaudit(record.id);
    message.success('反审核成功');
    actionRef.current?.reload();
  };

  const handleVoid = async (record: Voucher) => {
    await voucherApi.void(record.id);
    message.success('作废成功');
    actionRef.current?.reload();
  };

  const handleDelete = async (record: Voucher) => {
    await voucherApi.remove(record.id);
    message.success('删除成功');
    actionRef.current?.reload();
  };

  return (
    <ProTable<Voucher>
      actionRef={actionRef}
      headerTitle="凭证管理"
      rowKey="id"
      columns={columns}
      search={{ labelWidth: 'auto' }}
      request={createProTableRequest((params) => voucherApi.page(params as any))}
      toolBarRender={() => [
        <Button key="add" type="primary" icon={<PlusOutlined />} onClick={() => navigate('/account/voucher/add')}>新增凭证</Button>,
      ]}
      pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      rowSelection={{}}
    />
  );
};

export default VoucherList;
