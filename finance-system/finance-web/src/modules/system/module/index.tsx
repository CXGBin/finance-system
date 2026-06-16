import React, { useState } from 'react';
import { Card, Switch, Tag, Empty, Space, message } from 'antd';
import type { SysModule } from '@/types/system.d';
import { moduleApi } from '@/api/system';

/** 模块管理页面 */
const ModuleList: React.FC = () => {
  const [modules, setModules] = useState<SysModule[]>([]);
  const [loading, setLoading] = useState(false);

  const loadModules = async () => {
    setLoading(true);
    try {
      const data = await moduleApi.list();
      setModules(data.data ?? []);
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
    <Card title="模块管理" loading={loading}>
      {modules.length > 0 ? (
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
    </Card>
  );
};

export default ModuleList;
