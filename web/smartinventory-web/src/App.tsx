import { type FormEvent, useCallback, useEffect, useState } from 'react'
import './App.css'

type Product = {
  id: string
  sku: string
  name: string
  category: string
  unitPrice: number
  isActive: boolean
  createdAtUtc: string
}

const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5058').replace(/\/$/, '')

function App() {
  const [products, setProducts] = useState<Product[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)
  const [form, setForm] = useState({ sku: '', name: '', category: '', unitPrice: '' })

  const loadProducts = useCallback(async (term = '') => {
    try {
      const query = new URLSearchParams({ page: '1', pageSize: '100' })
      if (term.trim()) query.set('search', term.trim())
      const response = await fetch(`${apiBaseUrl}/api/v1/products?${query}`)
      if (!response.ok) throw new Error(`La API respondió ${response.status}.`)
      setProducts(await response.json() as Product[])
      setError('')
    } catch {
      setError(`No se pudo conectar con la API en ${apiBaseUrl}. Verifica que esté iniciada y permite el origen local en CORS.`)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    let cancelled = false
    queueMicrotask(() => { if (!cancelled) void loadProducts() })
    return () => { cancelled = true }
  }, [loadProducts])

  async function createProduct(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError('')
    try {
      const response = await fetch(`${apiBaseUrl}/api/v1/products`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...form, unitPrice: Number(form.unitPrice) }),
      })
      if (!response.ok) {
        const body = await response.json().catch(() => null) as { detail?: string } | null
        throw new Error(body?.detail ?? `La API respondió ${response.status}.`)
      }
      setForm({ sku: '', name: '', category: '', unitPrice: '' })
      await loadProducts(search)
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'No se pudo guardar el producto.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <main className="shell">
      <header className="topbar">
        <a className="brand" href="#inicio" aria-label="SmartInventory inicio">
          <span className="brand-mark">S</span>
          <span>smart<span className="brand-light">inventory</span></span>
        </a>
        <div className={`api-status ${error ? 'offline' : ''}`}><span className="status-dot" />{loading ? 'Conectando con API…' : error ? 'API sin conexión' : 'API conectada'}</div>
      </header>

      <section className="intro" id="inicio">
        <div>
          <p className="eyebrow">CONTROL DE INVENTARIO</p>
          <h1>Tu catálogo,<br />siempre en orden.</h1>
          <p className="intro-copy">Administra los productos desde una aplicación local conectada a una API ASP.NET Core.</p>
        </div>
        <div className="intro-card"><span className="intro-card-label">PRODUCTOS ACTIVOS</span><strong>{products.length.toString().padStart(2, '0')}</strong><span>en este catálogo</span></div>
      </section>

      <section className="workspace">
        <div className="catalog panel">
          <div className="section-heading">
            <div><p className="eyebrow">CATÁLOGO</p><h2>Productos</h2></div>
            <form className="search" onSubmit={(event) => { event.preventDefault(); setLoading(true); void loadProducts(search) }}>
              <input aria-label="Buscar productos" placeholder="Buscar SKU, nombre o categoría" value={search} onChange={(event) => setSearch(event.target.value)} />
              <button className="secondary-button" type="submit">Buscar</button>
            </form>
          </div>

          {error && <div className="alert" role="alert">{error}</div>}
          <div className="table-wrap">
            <table>
              <thead><tr><th>PRODUCTO</th><th>SKU</th><th>CATEGORÍA</th><th className="numeric">PRECIO UNITARIO</th></tr></thead>
              <tbody>
                {loading ? <tr><td className="empty" colSpan={4}>Consultando la API…</td></tr> : products.length === 0 ? <tr><td className="empty" colSpan={4}>No hay productos para mostrar. Registra el primero.</td></tr> : products.map((product) => (
                  <tr key={product.id}><td><strong>{product.name}</strong></td><td><span className="sku">{product.sku}</span></td><td>{product.category}</td><td className="numeric">S/ {product.unitPrice.toFixed(2)}</td></tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="table-footer">Mostrando {products.length} producto{products.length === 1 ? '' : 's'} · Conectado a {apiBaseUrl}</div>
        </div>

        <aside className="panel form-panel">
          <p className="eyebrow">NUEVO REGISTRO</p>
          <h2>Agregar producto</h2>
          <p className="form-copy">Los datos se envían a la API y se guardan en la base de datos.</p>
          <form className="product-form" onSubmit={createProduct}>
            <label>SKU<input required minLength={2} maxLength={32} placeholder="Ej. CAF-001" value={form.sku} onChange={(event) => setForm({ ...form, sku: event.target.value })} /></label>
            <label>Nombre del producto<input required minLength={2} maxLength={120} placeholder="Ej. Café molido 500 g" value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} /></label>
            <label>Categoría<input required minLength={2} maxLength={80} placeholder="Ej. Abarrotes" value={form.category} onChange={(event) => setForm({ ...form, category: event.target.value })} /></label>
            <label>Precio unitario (S/)<input required type="number" min="0" step="0.01" placeholder="0.00" value={form.unitPrice} onChange={(event) => setForm({ ...form, unitPrice: event.target.value })} /></label>
            <button className="primary-button" type="submit" disabled={saving}>{saving ? 'Guardando…' : 'Guardar producto'}<span aria-hidden="true">↗</span></button>
          </form>
        </aside>
      </section>
      <footer>SmartInventory · Proyecto de demostración · No procesa pagos</footer>
    </main>
  )
}

export default App
