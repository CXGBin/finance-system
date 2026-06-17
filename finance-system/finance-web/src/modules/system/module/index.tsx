import React, { useState } from 'react';
import { Card, Switch, Tag, Empty, Space, message, Spin, Alert, Button } from 'antd';
import type { SysModule } from '@/types/system.d';
import { moduleApi } from '@/api/system';

/** 模块管理页面 */
const ModuleList: React.FC = () => {
  const [modules, setModules] = useState<SysModule[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadModules = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await moduleApi.list();
      setModules(data.data ?? []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载模块数据失败');
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => { loadModules(); }, []);

  const handleToggle = async (module: SysModule, enabled: boolean) => {
    await moduleApi.toggle(module.moduleId, enabled);
    message.success(enabled ? '已启用' : '已停用');
    loadModules();
  };

  return (
    <Card title="模块管理">
      <Spin spinning={loading}>
        {error ? (
          <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadModules}>重试</Button>} />
        ) : modules.length > 0 ? (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: 16 }}>
            {modules.map((m) => (
              <Card key={m.moduleId} size="small" title={m.moduleName}
                extra={<Switch checked={m.isEnabled} checkedChildren="启用" unCheckedChildren="禁用"
                  onChange={(checked) => handleToggle(m, checked)} />}>
              <p style={{ margin: '4px 0', color: '#666' }}>{m.description || '暂无描述'}</p>
              <Space>
                <Tag color={m.isCore ? 'blue' : 'default'}>{m.isCore ? '核心' : '扩展'}</Tag>
                {m.moduleId && <Tag>{m.moduleId}</Tag>}
              </Space>
            </Card>
          ))}
        </div>
      ) : (
          <Empty description="暂无模块数据" />
        )}
      </Spin>
    </Card>
  );
};

export default ModuleList;
