import { Route, Routes, Navigate } from 'react-router-dom';
import './App.css';
import Chat from './components/chat/chat';
import ChatCreation from './components/chatCreation/chatCreation'

function App() {
  return (
    <Routes>
      <Route path='/' element={<Navigate to='chat' replace />} />
      <Route path='chat' element={<ChatCreation />} />
      <Route path='chat/:id' element={<Chat />} />
    </Routes>
  )
}

export default App;
