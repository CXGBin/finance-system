import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Table, Tag, Button, Space, message } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { voucherApi } from '@/api/account';
import type { Voucher } from '@/types/account.d';

/** 凭证详情页面 */
const VoucherDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [voucher, setVoucher] = useState<Voucher | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) loadDetail(Number(id));
  }, [id]);

  const loadDetail = async (vid: number) => {
    setLoading(true);
    try {
      const res = await voucherApi.detail(vid);
      setVoucher(res.data || null);
    } finally { setLoading(false); }
  };

  const handleAudit = async () => {
    if (!id) return;
    await voucherApi.audit(Number(id));
    message.success('审核成功');
    loadDetail(Number(id));
  };

  const statusMap: Record<number, { color: string; text: string }> = {
    0: { color: 'default', text: '草稿' }, 1: { color: 'processing', text: '待审核' },
    2: { color: 'success', text: '已审核' }, 3: { color: 'error', text: '已作废' },
  };

  const entryColumns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode' },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '摘要', dataIndex: 'summary', key: 'summary' },
    { title: '借方金额', dataIndex: 'debit', key: 'debit', align: 'right' },
    { title: '贷方金额', dataIndex: 'credit', key: 'credit', align: 'right' },
  ];

  return (
    <Card title="凭证详情" loading={loading} extra={
      <Space>
        {voucher?.status === 1 && <Button type="primary" onClick={handleAudit}>审核</Button>}
        <Button onClick={() => navigate(-1)}>返回</Button>
      </Space>
    }>
      {voucher && (
        <>
          <Descriptions bordered size="small" column={4} style={{ marginBottom: 16 }}>
            <Descriptions.Item label="凭证字号">{voucher.voucherNo}</Descriptions.Item>
            <Descriptions.Item label="凭证日期">{voucher.voucherDate}</Descriptions.Item>
            <Descriptions.Item label="状态"><Tag color={statusMap[voucher.status]?.color}>{statusMap[voucher.status]?.text}</Tag></Descriptions.Item>
            <Descriptions.Item label="制单人">{voucher.createBy}</Descriptions.Item>
          </Descriptions>
          <Table columns={entryColumns} dataSource={voucher.entries || []} rowKey="id" pagination={false}
            footer={() => (
              <Space style={{ float: 'right' }}>
                <span>借方合计: {voucher.totalDebit?.toFixed(2)}</span>
                <span>贷方合计: {voucher.totalCredit?.toFixed(2)}</span>
              </Space>
            )}
          />
        </>
      )}
    </Card>
  );
};

export default VoucherDetail;
