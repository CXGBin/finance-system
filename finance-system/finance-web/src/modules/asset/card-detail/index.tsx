import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Button, Statistic, Row, Col, message, Spin, Alert, Empty } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { assetApi } from '@/api/asset';
import type { AssetCard } from '@/types/asset.d';

/** 资产详情 */
const AssetCardDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<AssetCard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => { if (id) loadData(Number(id)); }, [id]);

  const loadData = async (vid: number) => {
    setLoading(true);
    setError(null);
    try { const res = await assetApi.cardDetail(vid); setData(res.data || null); } catch (err: unknown) { setError(err instanceof Error ? err.message : '加载资产详情失败'); } finally { setLoading(false); }
  };

  return (
    <Card title="资产详情" extra={<Button onClick={() => navigate(-1)}>返回</Button>}>
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={() => id && loadData(Number(id))}>重试</Button>} />
        ) : data ? (<>
          <Descriptions bordered size="small" column={3}>
            <Descriptions.Item label="资产编号">{data.assetCode}</Descriptions.Item>
            <Descriptions.Item label="资产名称">{data.assetName}</Descriptions.Item>
            <Descriptions.Item label="状态">{['', '在用', '闲置', '维修中', '已处置', '已报废'][data.status]}</Descriptions.Item>
            <Descriptions.Item label="资产原值">{data.originalValue?.toFixed(2)}</Descriptions.Item>
            <Descriptions.Item label="残值率">{data.residualRate}%</Descriptions.Item>
            <Descriptions.Item label="使用年限">{data.usefulLifeMonths} 个月</Descriptions.Item>
            <Descriptions.Item label="存放地点">{data.location}</Descriptions.Item>
            <Descriptions.Item label="保管人">{data.keeper}</Descriptions.Item>
            <Descriptions.Item label="入账日期">{data.acquisitionDate}</Descriptions.Item>
          </Descriptions>
          <Row gutter={16} style={{ marginTop: 16 }}>
            <Col span={8}><Statistic title="累计折旧" value={data.accumulatedDepreciation} precision={2} /></Col>
            <Col span={8}><Statistic title="净值" value={data.netValue} precision={2} /></Col>
            <Col span={8}><Statistic title="已折旧月数" value={Math.round(data.accumulatedDepreciation / (data.originalValue - data.residualValue) * data.usefulLifeMonths)} suffix="个月" /></Col>
          </Row>
        </>) : !loading ? (
          <Empty description="暂无资产数据" />
        ) : null}
      </Spin>
  );
};

export default AssetCardDetail;
