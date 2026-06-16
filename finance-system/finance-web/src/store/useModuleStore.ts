import { create } from 'zustand';

interface ModuleState {
  modules: string[]; // 后端返回模块标识字符串列表
  setModules: (modules: string[]) => void;
  isModuleEnabled: (code: string) => boolean;
}

export const useModuleStore = create<ModuleState>((set, get) => ({
  modules: [],
  setModules: (modules) => set({ modules }),
  isModuleEnabled: (code) => {
    return get().modules.includes(code);
  },
}));
