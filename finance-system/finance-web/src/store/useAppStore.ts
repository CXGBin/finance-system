import { create } from 'zustand';

export interface TabItem {
  key: string;
  title: string;
  closable: boolean;
  pathname: string;
}

interface AppState {
  sidebarCollapsed: boolean;
  toggleSidebar: () => void;
  setSidebarCollapsed: (collapsed: boolean) => void;
  // 多页签
  tabs: TabItem[];
  activeTabKey: string;
  addTab: (tab: TabItem) => void;
  removeTab: (key: string) => void;
  setActiveTab: (key: string) => void;
  clearOtherTabs: (key: string) => void;
}

export const useAppStore = create<AppState>((set, get) => ({
  sidebarCollapsed: false,
  toggleSidebar: () => set((s) => ({ sidebarCollapsed: !s.sidebarCollapsed })),
  setSidebarCollapsed: (collapsed) => set({ sidebarCollapsed: collapsed }),
  // 多页签
  tabs: [{ key: '/dashboard', title: '首页', closable: false, pathname: '/dashboard' }],
  activeTabKey: '/dashboard',
  addTab: (tab) => {
    const { tabs } = get();
    if (tabs.some((t) => t.key === tab.key)) return;
    set({ tabs: [...tabs, tab] });
  },
  removeTab: (key) => {
    const { tabs, activeTabKey } = get();
    const newTabs = tabs.filter((t) => t.key !== key);
    let newActive = activeTabKey;
    if (activeTabKey === key) {
      const idx = tabs.findIndex((t) => t.key === key);
      newActive = newTabs[Math.min(idx, newTabs.length - 1)]?.key || '/dashboard';
    }
    set({ tabs: newTabs, activeTabKey: newActive });
  },
  setActiveTab: (key) => set({ activeTabKey: key }),
  clearOtherTabs: (key) => {
    const { tabs } = get();
    set({ tabs: tabs.filter((t) => t.key === key || t.key === '/dashboard' || !t.closable) });
  },
}));
