import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Table, Tag, Timeline, Button, Space, message, Input } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { approvalApi } from '@/api/approval';
import type { ApprovalInstance } from '@/types/approval.d';

/** 审批详情 */
const ApprovalDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<ApprovalInstance | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => { if (id) loadData(Number(id)); }, [id]);

  const loadData = async (vid: number) => {
    setLoading(true);
    try { const res = await approvalApi.detail(vid); setData(res.data || null); } finally { setLoading(false); }
  };

  const handleApprove = async () => { await approvalApi.approve(Number(id!), '同意'); message.success('已通过'); loadData(Number(id!)); };
  const handleReject = async () => { await approvalApi.reject(Number(id!), '不同意'); message.success('已驳回'); loadData(Number(id!)); };

  return (
    <Card title="审批详情" loading={loading} extra={<Space>
      <Button type="primary" onClick={handleApprove}>通过</Button>
      <Button danger onClick={handleReject}>驳回</Button>
      <Button onClick={() => navigate(-1)}>返回</Button>
    </Space>}>
      {data && (
        <>
          <Descriptions bordered size="small" column={3} style={{ marginBottom: 16 }}>
            <Descriptions.Item label="审批单号">{data.approvalNo}</Descriptions.Item>
            <Descriptions.Item label="标题">{data.title}</Descriptions.Item>
            <Descriptions.Item label="状态"><Tag>{data.status === 0 ? '审批中' : data.status === 1 ? '已通过' : '已驳回'}</Tag></Descriptions.Item>
            <Descriptions.Item label="发起人">{data.initiatorName}</Descriptions.Item>
            <Descriptions.Item label="发起时间">{data.createdTime}</Descriptions.Item>
          </Descriptions>
          <h4>审批记录</h4>
          <Timeline items={((data as { records?: Array<{ operatorName: string; action: string; comment?: string; handleTime: string }> }).records || []).map((r) => ({ children: `${r.operatorName} - ${r.action} - ${r.comment || '无备注'} (${r.handleTime})` }))} />
        </>
      )}
    </Card>
  );
};

export default ApprovalDetail;
