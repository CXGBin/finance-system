import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Table, Tag, Button, Space, message, Modal, Spin, Alert, Empty } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { voucherApi, voucherBatchApi } from '@/api/account';
import type { Voucher } from '@/types/account.d';

/** 凭证详情页面 */
const VoucherDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [voucher, setVoucher] = useState<Voucher | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [printVisible, setPrintVisible] = useState(false);
  const [printData, setPrintData] = useState<Record<string, unknown> | null>(null);

  useEffect(() => {
    if (id) loadDetail(Number(id));
  }, [id]);

  const loadDetail = async (vid: number) => {
    setLoading(true);
    setError(null);
    try {
      const res = await voucherApi.detail(vid);
      setVoucher(res.data || null);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载凭证详情失败');
    } finally { setLoading(false); }
  };

  const handleAudit = async () => {
    if (!id) return;
    await voucherApi.audit(Number(id));
    message.success('审核成功');
    loadDetail(Number(id));
  };

  /** 打开打印预览 */
  const handlePrint = async () => {
    if (!id) return;
    try {
      const res = await voucherBatchApi.printData(Number(id));
      setPrintData(res.data || null);
      setPrintVisible(true);
    } catch {
      message.error('获取打印数据失败');
    }
  };

  const statusMap: Record<number, { color: string; text: string }> = {
    0: { color: 'default', text: '草稿' }, 1: { color: 'processing', text: '待审核' },
    2: { color: 'success', text: '已审核' }, 3: { color: 'error', text: '已作废' },
  };

  const entryColumns = [
    { title: '科目编码', dataIndex: 'subjectCode', key: 'subjectCode' },
    { title: '科目名称', dataIndex: 'subjectName', key: 'subjectName' },
    { title: '摘要', dataIndex: 'summary', key: 'summary' },
    { title: '借方金额', dataIndex: 'debitAmount', key: 'debitAmount', align: 'right' },
    { title: '贷方金额', dataIndex: 'creditAmount', key: 'creditAmount', align: 'right' },
  ];

  return (
    <Card title="凭证详情" extra={
      <Space>
        {voucher?.status === 1 && <Button type="primary" onClick={handleAudit}>审核</Button>}
        <Button onClick={handlePrint}>打印</Button>
        <Button onClick={() => navigate(-1)}>返回</Button>
      </Space>
    }>
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={() => id && loadDetail(Number(id))}>重试</Button>} />
        ) : voucher ? (
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
        ) : !loading ? (
          <Empty description="暂无凭证数据" />
        ) : null}
      </Spin>
      <Modal
        title="凭证打印预览"
        open={printVisible}
        onCancel={() => setPrintVisible(false)}
        footer={[
          <Button key="cancel" onClick={() => setPrintVisible(false)}>关闭</Button>,
          <Button key="print" type="primary" onClick={() => window.print()}>打印</Button>,
        ]}
        width={700}
      >
        {printData && (
          <div className="print-content">
            <h3 style={{ textAlign: 'center' }}>记 账 凭 证</h3>
            <p>凭证字号：{String(printData.voucherNo)}　日期：{String(printData.voucherDate)}</p>
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr>
                  <th style={{ border: '1px solid #ddd', padding: 8 }}>摘要</th>
                  <th style={{ border: '1px solid #ddd', padding: 8 }}>科目</th>
                  <th style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right' }}>借方金额</th>
                  <th style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right' }}>贷方金额</th>
                </tr>
              </thead>
              <tbody>
                {((printData.entries || []) as Array<Record<string, string | number>>).map((entry, i) => (
                  <tr key={i}>
                    <td style={{ border: '1px solid #ddd', padding: 8 }}>{String(entry.summary || '')}</td>
                    <td style={{ border: '1px solid #ddd', padding: 8 }}>{String(entry.subjectName || '')}</td>
                    <td style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right' }}>{Number(entry.debitAmount || entry.debit || 0).toFixed(2)}</td>
                    <td style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right' }}>{Number(entry.creditAmount || entry.credit || 0).toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr>
                  <td colSpan={2} style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right', fontWeight: 'bold' }}>合计</td>
                  <td style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right', fontWeight: 'bold' }}>{Number(printData.totalDebit || 0).toFixed(2)}</td>
                  <td style={{ border: '1px solid #ddd', padding: 8, textAlign: 'right', fontWeight: 'bold' }}>{Number(printData.totalCredit || 0).toFixed(2)}</td>
                </tr>
              </tfoot>
            </table>
          </div>
        )}
      </Modal>
    </Card>
  );
};

export default VoucherDetail;
