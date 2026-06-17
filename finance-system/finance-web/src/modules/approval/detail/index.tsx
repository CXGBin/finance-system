import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Table, Tag, Timeline, Button, Space, message, Modal, Select, Spin, Alert, Empty } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { approvalApi } from '@/api/approval';
import type { ApprovalInstance } from '@/types/approval.d';

/** 审批详情 */
const ApprovalDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<ApprovalInstance | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [transferVisible, setTransferVisible] = useState(false);
  const [transferUserId, setTransferUserId] = useState<number | undefined>(undefined);
  const [transferComment, setTransferComment] = useState('');
  const [userOptions, setUserOptions] = useState<Array<{ label: string; value: number }>>([]);

  useEffect(() => { if (id) loadData(Number(id)); }, [id]);

  const loadData = async (vid: number) => {
    setLoading(true);
    setError(null);
    try { const res = await approvalApi.detail(vid); setData(res.data || null); } catch (err: unknown) { setError(err instanceof Error ? err.message : '加载审批详情失败'); } finally { setLoading(false); }
  };

  const handleApprove = async () => { await approvalApi.approve(Number(id!), '同意'); message.success('已通过'); loadData(Number(id!)); };
  const handleReject = async () => { await approvalApi.reject(Number(id!), '不同意'); message.success('已驳回'); loadData(Number(id!)); };

  /** 打开转审弹窗 */
  const handleOpenTransfer = async () => {
    setTransferVisible(true);
    setTransferUserId(undefined);
    setTransferComment('');
    // 加载可选审批人列表
    try {
      const res = await approvalApi.list({ pageIndex: 1, pageSize: 100, type: 'approvers' } as any);
      const users = ((res.data as any)?.list || []).map((u: any) => ({ label: u.approverName || u.name, value: u.id || u.userId }));
      setUserOptions(users);
    } catch {
      setUserOptions([]);
    }
  };

  /** 确认转审 */
  const handleTransfer = async () => {
    if (!transferUserId) {
      message.warning('请选择目标审批人');
      return;
    }
    try {
      await approvalApi.transfer(Number(id!), transferUserId, transferComment || '转审');
      message.success('转审成功');
      setTransferVisible(false);
      loadData(Number(id!));
    } catch {
      message.error('转审失败');
    }
  };

  return (
    <Card title="审批详情" extra={<Space>
      <Button type="primary" onClick={handleApprove}>通过</Button>
      <Button danger onClick={handleReject}>驳回</Button>
      <Button onClick={handleOpenTransfer}>转审</Button>
      <Button onClick={() => navigate(-1)}>返回</Button>
    </Space>}>
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={() => id && loadData(Number(id))}>重试</Button>} />
        ) : data ? (
          <>
            <Descriptions bordered size="small" column={3} style={{ marginBottom: 16 }}>
              <Descriptions.Item label="业务ID">{data.businessId}</Descriptions.Item>
              <Descriptions.Item label="标题">{data.title}</Descriptions.Item>
              <Descriptions.Item label="状态"><Tag>{data.status === 0 ? '审批中' : data.status === 1 ? '已通过' : '已驳回'}</Tag></Descriptions.Item>
              <Descriptions.Item label="发起人ID">{data.initiatorId}</Descriptions.Item>
              <Descriptions.Item label="发起时间">{data.createdTime}</Descriptions.Item>
            </Descriptions>
            <h4>审批记录</h4>
            <Timeline items={((data as { records?: Array<{ operatorName: string; action: string; comment?: string; handleTime: string }> }).records || []).map((r) => ({ children: `${r.operatorName} - ${r.action} - ${r.comment || '无备注'} (${r.handleTime})` }))} />
          </>
        ) : !loading ? (
          <Empty description="暂无审批数据" />
        ) : null}
      </Spin>
      <Modal
        title="转审"
        open={transferVisible}
        onOk={handleTransfer}
        onCancel={() => setTransferVisible(false)}
      >
        <div style={{ marginBottom: 16 }}>
          <div style={{ marginBottom: 8, fontWeight: 'bold' }}>选择目标审批人</div>
          <Select
            value={transferUserId}
            onChange={setTransferUserId}
            style={{ width: '100%' }}
            placeholder="请选择审批人"
            options={userOptions}
          />
        </div>
        <div>
          <div style={{ marginBottom: 8, fontWeight: 'bold' }}>转审备注</div>
          <Select
            value={transferComment || undefined}
            onChange={setTransferComment}
            style={{ width: '100%' }}
            placeholder="请选择转审原因"
            options={[
              { label: '业务需要', value: '业务需要' },
              { label: '权限不足', value: '权限不足' },
              { label: '转交他人处理', value: '转交他人处理' },
            ]}
          />
        </div>
      </Modal>
    </Card>
  );
};

export default ApprovalDetail;
