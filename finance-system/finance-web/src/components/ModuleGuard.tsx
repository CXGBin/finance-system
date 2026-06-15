import React from 'react';
import { Result } from 'antd';
import { useModuleStore } from '@/store/useModuleStore';

interface ModuleGuardProps {
  moduleCode: string;
  children: React.ReactNode;
}

const ModuleGuard: React.FC<ModuleGuardProps> = ({ moduleCode, children }) => {
  const isModuleEnabled = useModuleStore((s) => s.isModuleEnabled);

  if (!isModuleEnabled(moduleCode)) {
    return (
      <Result
        status="403"
        title="模块未启用"
        subTitle={`模块 ${moduleCode} 当前未开启，请联系管理员启用。`}
      />
    );
  }

  return <>{children}</>;
};

export default ModuleGuard;
