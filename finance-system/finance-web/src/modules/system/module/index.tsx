import React, { useEffect, useState } from 'react';
import { Table, Switch, Tag, message } from 'antd';
import type { SysModule } from '@/types/system.d';
import { moduleApi } from '@/api/system';

/** 模块管理页面 */
const ModuleList: React.FC = () => {
  const [modules, setModules] = useState<SysModule[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => { loadModules(); }, []);

  const loadModules = async () => {
    setLoading(true);
    const res = await moduleApi.list();
    setModules(res.data || []);
    setLoading(false);
  };

  const handleToggle = async (record: SysModule) => {
    if (record.isCore) {
      message.warning('核心模块不可关闭');
      return;
    }
    await moduleApi.update(record.id, !record.isEnabled);
    message.success(record.isEnabled ? '已关闭' : '已启用');
    loadModules();
  };

  const columns = [
    { title: '模块标识', dataIndex: 'moduleId', key: 'moduleId' },
    { title: '模块名称', dataIndex: 'moduleName', key: 'moduleName' },
    { title: '排序', dataIndex: 'sortOrder', key: 'sortOrder' },
    { title: '描述', dataIndex: 'description', key: 'description', ellipsis: true },
    {
      title: '状态', dataIndex: 'isEnabled', key: 'isEnabled',
      render: (val: number, record: SysModule) => (
        record.isCore ? <Tag color="blue">核心</Tag> : (
          <Switch checked={val === 1} onChange={() => handleToggle(record as SysModule)} />
        )
      ),
    },
  ];

  return <Table dataSource={modules} columns={columns} rowKey="id" loading={loading} pagination={false} />;
};

export default ModuleList;
